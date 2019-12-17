import { Component, OnInit, ChangeDetectorRef, NgZone } from '@angular/core';
import { ScheduleService } from './services/schedule.service';
import { Schedule } from '../models/schedule.model';
import { AlertController, ToastController, ModalController } from '@ionic/angular';
import { Zone } from '../models/zone.model';
import { ModalZonePage } from './modal-zone/modal-zone.page';
import { ZoneForUpdate } from '../models/zoneForUpdate.model';

@Component({
  selector: 'app-schedule',
  templateUrl: 'schedule.page.html',
  styleUrls: ['schedule.page.scss']
})
export class SchedulePage implements OnInit {

  // tslint:disable-next-line: new-parens
  public schedule: Schedule = new Schedule();
  public systemId = 1012;
  public dualKnobs = { lower: 15, upper: 15 };

  public zones: Zone[];

  constructor(
    public scheduleService: ScheduleService,
    private alertController: AlertController,
    public toastController: ToastController,
    public modalController: ModalController
  ) { }

  ngOnInit() {
  }

  ionViewWillEnter() {
    this.scheduleService.GetSchedule(this.systemId).subscribe(sch => {
      this.schedule = sch;
      console.log(this.schedule);
      this.dualKnobs = { lower: this.schedule.temperatureMin, upper: this.schedule.temperatureMax };
      // this.schedule.start.
    });

    this.scheduleService.GetZones(this.systemId).subscribe(zone => {
      this.zones = zone;
      console.log(this.zones);
    });
  }

  async OpenModal() {
    const modal = await this.modalController.create({
      component: ModalZonePage
    });
    return await modal.present();
  }

  UpdateSchedule() {
    this.scheduleService.UpdateSchedule(this.systemId, this.schedule).subscribe(
      x => console.log('Observer got a next value: ' + x),
      err => this.presentToast('An error occured. Try again later'),
      () => this.presentToast('Succefully updated'));
  }

  async presentToast(message: string) {
    const toast = await this.toastController.create({
      message,
      duration: 2000,
      buttons: [
        {
          text: 'Close',
          role: 'cancel',
        }
      ]
    }
    );
    toast.present();
  }

  UpdateZoneMoisture(systemId: number, zoneId: number, zone: ZoneForUpdate) {
    this.scheduleService.UpdateMoisture(systemId, zoneId, zone).subscribe(
      x => console.log('Observer got a next value: ' + x),
      err => this.presentToast('An error occured. Try again later'),
      () => this.presentToast('Succefully updated')
    );
  }

  async presentMoistureAlertConfirm(zoneId: number, zone: Zone) {
    const alert = await this.alertController.create({
      header: 'Confirm!',
      message: 'Change moisture range?',
      buttons: [
        {
          text: 'Cancel',
          role: 'cancel',
          cssClass: 'secondary',
        }, {
          text: 'Okay',
          handler: () => {
            const zoneForUpdate = {
              moistureStart: zone.moistureStart,
              moistureStop: zone.moistureStop
            };

            this.UpdateZoneMoisture(this.systemId, zoneId, zoneForUpdate);
            console.log('moisture updates');
          }
        }
      ]
    });
    await alert.present();
  }


  async presentTemperatureAlertConfirm() {
    const alert = await this.alertController.create({
      header: 'Confirm!',
      message: 'Change temperature range?',
      buttons: [
        {
          text: 'Cancel',
          role: 'cancel',
          cssClass: 'secondary',
        }, {
          text: 'Okay',
          handler: () => {
            this.schedule.temperatureMin = this.dualKnobs.lower;
            this.schedule.temperatureMax = this.dualKnobs.upper;
            this.UpdateSchedule();
            console.log('temp updates');
          }
        }
      ]
    });
    await alert.present();
  }

  DeleteZone(zoneId: number) {
    this.scheduleService.DeleteZone(this.systemId, zoneId).subscribe(
      x => console.log('Observer got a next value: ' + x),
      err => this.presentToast('An error occured. Try again later'),
      () => {
        this.presentToast('Succefully deleted');
      });
  }

  async presentAlertDeleteZone(zone: Zone) {
    const alert = await this.alertController.create({
      header: 'Confirm!',
      message: 'Delete ' + zone.name + ' zone?',
      buttons: [
        {
          text: 'No',
          role: 'cancel',
          cssClass: 'secondary',
        }, {
          text: 'Yes',
          handler: () => {
            this.DeleteZone(zone.id);
            const index = this.zones.indexOf(zone, 0);
            if (index > -1) {
              this.zones.splice(index, 1);
            }
            console.log(zone.id);
          }
        }
      ]
    });
    await alert.present();
  }
}
