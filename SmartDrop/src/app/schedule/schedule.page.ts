import { Component, OnInit, ChangeDetectorRef, NgZone } from '@angular/core';
import { ScheduleService } from './services/schedule.service';
import { Schedule } from '../models/schedule.model';
import { AlertController, ToastController, ModalController } from '@ionic/angular';
import { Zone } from '../models/zone.model';
import { ModalZonePage } from './modal-zone/modal-zone.page';
import { ZoneForUpdate } from '../models/zoneForUpdate.model';
import { Storage } from '@ionic/storage';
import { EditMoistureModalComponent } from './edit-moisture-modal/edit-moisture-modal.component';
@Component({
  selector: 'app-schedule',
  templateUrl: 'schedule.page.html',
  styleUrls: ['schedule.page.scss']
})
export class SchedulePage implements OnInit {

  // tslint:disable-next-line: new-parens
  public schedule: Schedule = new Schedule();
  public systemId;

  public dualKnobs = { lower: 15, upper: 15 };
  public temperatureUpdatedState = true;

  public zones: Zone[];


  constructor(
    public scheduleService: ScheduleService,
    private alertController: AlertController,
    public toastController: ToastController,
    public modalController: ModalController,
    public storage: Storage,
  ) { }

  ngOnInit() {

  }

  ionViewWillEnter() {
    this.storage.get('irrigationSystemId').then(id => {
      this.systemId = id;
      this.getSchedule();
      this.getZones();
      console.log(this.systemId);
    });
  }

  getZones() {
    this.scheduleService.GetZones(this.systemId).subscribe(zone => {
      this.zones = zone.map(item => {
        return {
          ...item,
          changeState: true
        };
      });
      console.log(this.zones);
    });
  }

  getSchedule() {
    this.scheduleService.GetSchedule(this.systemId).subscribe(sch => {
      this.schedule = sch;
      console.log(this.schedule);
      this.dualKnobs = { lower: this.schedule.temperatureMin, upper: this.schedule.temperatureMax };
      // this.schedule.start.
    });
  }

  async OpenModal() {
    const modal = await this.modalController.create({
      component: ModalZonePage
    });
    modal.onDidDismiss()
      .then((data) => {
        this.getZones();
      });
    return await modal.present();
  }

  UpdateSchedule() {
    if (this.systemId !== null && this.schedule !== null) {
      this.scheduleService.UpdateSchedule(this.systemId, this.schedule).subscribe(
        x => console.log('Observer got a next value: ' + x),
        err => this.presentToast('A aparut o eroare. Incercati mai tarziu'),
        () => this.presentToast('Program actualizat cu succes'));
    }
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
      err => this.presentToast('A aparut o eroare. Incercati din nou'),
      () => {
        this.presentToast(zone.waterSwitch ? 'Electrovalva a fost deschisa' : 'Electrovalva a fost inchisa');
      }
    );
  }

  updateSwitchState(zoneId: number, zone: Zone) {
    const zoneForUpdate = {
      moistureStart: zone.moistureStart,
      moistureStop: zone.moistureStop,
      waterSwitch: zone.waterSwitch,
      systemId: this.systemId
    };

    this.UpdateZoneMoisture(this.systemId, zoneId, zoneForUpdate);
  }

  async presentTemperatureUpdateAlertConfirm() {
    const alert = await this.alertController.create({
      header: 'Confirmati!',
      message: 'Schimbarea intervalului temperaturii?',
      buttons: [
        {
          text: 'Cancel',
          role: 'cancel',
          cssClass: 'secondary',
          handler: () => {
            this.getSchedule();
            this.temperatureUpdatedState = true;
          }
        }, {
          text: 'Okay',
          handler: () => {
            this.schedule.temperatureMin = this.dualKnobs.lower;
            this.schedule.temperatureMax = this.dualKnobs.upper;
            this.UpdateSchedule();
            this.temperatureUpdatedState = true;
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
        this.presentToast('Senzor sters cu succes');
      });
  }

  async presentAlertDeleteZone(zone: Zone) {
    const alert = await this.alertController.create({
      header: 'Confirmati stergerea!',
      message: 'Doriti sa stergeti senzorul ' + ' ' + zone.name + '?',
      buttons: [
        {
          text: 'Nu',
          role: 'cancel',
          cssClass: 'secondary',
        }, {
          text: 'Da',
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

  updateMoistureConfirmation(zone: Zone) {
    zone.changeState = false;
  }

  updateTemperatureConfirmation() {
    this.temperatureUpdatedState = false;
  }

  async openEditModal(zone: Zone) {
    const modal = await this.modalController.create({
      component: EditMoistureModalComponent,
      componentProps: {
        moistureStart: zone.moistureStart,
        moistureStop: zone.moistureStop,
        waterSwitch: zone.waterSwitch,
        zoneId: zone.id,
        isAllZoneEdit: false,
        systemId: this.systemId
      }
    },
    );
    modal.onDidDismiss().then((data) => {
      this.getZones();
    });
    await modal.present();
  }


  // restrictMoisture(zone: Zone) {
  //   if (zone.moistureStart >= zone.moistureStop) {
  //     zone.moistureStart = zone.moistureStop;
  //     console.log(zone.moistureStart);
  //     console.log('triggered');
  //   }
  // }
}
