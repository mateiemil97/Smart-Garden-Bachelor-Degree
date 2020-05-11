import { Component, OnInit } from '@angular/core';
import { ZoneForCreate } from 'src/app/models/zoneForCreate.model';
import { ZoneForUpdate } from 'src/app/models/zoneForUpdate.model';
import { NavParams, ModalController, ToastController } from '@ionic/angular';
import { ScheduleService } from '../services/schedule.service';

@Component({
  selector: 'app-edit-moisture-modal',
  templateUrl: './edit-moisture-modal.component.html',
  styleUrls: ['./edit-moisture-modal.component.scss'],
})
export class EditMoistureModalComponent implements OnInit {

  zoneForUpdate: ZoneForUpdate = new ZoneForUpdate();
  isAllZoneEdit: boolean;
  zoneId: number;
  systemId: number;
  constructor(
    public navParams: NavParams,
    public modal: ModalController,
    public scheduleService: ScheduleService,
    public toastController: ToastController
   ) { }

  ngOnInit() {
  }

  ionViewWillEnter() {
    this.zoneForUpdate.moistureStart = this.navParams.get('moistureStart');
    this.zoneForUpdate.moistureStop = this.navParams.get('moistureStop');
    this.zoneForUpdate.waterSwitch = this.navParams.get('waterSwitch');
    this.zoneId = this.navParams.get('zoneId');
    this.systemId = this.navParams.get('systemId');
    console.log(this.zoneForUpdate.moistureStart);
    console.log(this.zoneForUpdate.moistureStop);
  }

  editMoisture() {
    if (!this.isAllZoneEdit) {
      this.updateMoistureForOneZone();
    }
  }

  updateMoistureForOneZone() {
    const zoneForUpdate = {
      moistureStart: this.zoneForUpdate.moistureStart,
      moistureStop: this.zoneForUpdate.moistureStop,
      waterSwitch: this.zoneForUpdate.waterSwitch
    };

    this.UpdateZoneMoisture(this.systemId, this.zoneId, zoneForUpdate);
  }

  UpdateZoneMoisture(systemId: number, zoneId: number, zone: ZoneForUpdate) {
    this.scheduleService.UpdateMoisture(systemId, zoneId, zone).subscribe(
      x => console.log('Observer got a next value: ' + x),
      err => this.presentToast('A aparut o eroare. Incercati din nou'),
      () => {
        this.presentToast('Umiditate actualizata cu succes');
      }
    );
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

  dismissModal() {
    this.modal.dismiss();
  }

}
