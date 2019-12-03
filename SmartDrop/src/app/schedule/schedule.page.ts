import { Component, OnInit, ChangeDetectorRef, NgZone } from '@angular/core';
import { ScheduleService } from './services/schedule.service';
import { Schedule } from '../models/schedule.model';
import { AlertController, ToastController, ModalController } from '@ionic/angular';
import { Zone } from '../models/zone.model';
import { ModalZoneComponent } from './modal-zone/modal-zone.component';

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

  public zones: Zone;

  constructor(
    public scheduleService: ScheduleService,
    private alertController: AlertController,
    public toastController: ToastController,
    public modalController: ModalController
  ) { }

  ngOnInit() {
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
      component: ModalZoneComponent
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

  async presentAlertConfirm() {
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
}