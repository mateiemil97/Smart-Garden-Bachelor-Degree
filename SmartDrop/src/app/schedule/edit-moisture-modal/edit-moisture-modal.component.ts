import { Component, OnInit } from '@angular/core';
import { ZoneForCreate } from 'src/app/models/zoneForCreate.model';
import { ZoneForUpdate } from 'src/app/models/zoneForUpdate.model';
import { NavParams, ModalController, ToastController } from '@ionic/angular';
import { ScheduleService } from '../services/schedule.service';
import { VegetablesServiceService } from 'src/app/vegetables/services/vegetables-service.service';
import {Storage} from '@ionic/storage';
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
  userId: number;
  vegetableId: number;

  constructor(
    public navParams: NavParams,
    public modal: ModalController,
    public scheduleService: ScheduleService,
    public toastController: ToastController,
    public vegetableService: VegetablesServiceService,
    public storage: Storage
  ) { }

  ngOnInit() {

  }

  ionViewWillEnter() {
    this.zoneForUpdate.moistureStart = this.navParams.get('moistureStart');
    this.zoneForUpdate.moistureStop = this.navParams.get('moistureStop');
    this.zoneForUpdate.waterSwitch = this.navParams.get('waterSwitch');
    this.zoneId = this.navParams.get('zoneId');
    this.isAllZoneEdit = this.navParams.get('isAllZoneEdit');
    this.userId = this.navParams.get('userId');
    this.vegetableId = this.navParams.get('vegetableId');

    this.storage.get('irrigationSystemId').then(item => {
      this.systemId = item;
      console.log(item);
    });
  }

    editMoisture() {
      if (!this.isAllZoneEdit) {
        this.updateMoistureForOneZone();
      } else {
        this.updateMultipleZonesMoisture(this.vegetableId);
      }
    }

    updateMoistureForOneZone() {
      const zoneForUpdate = {
        moistureStart: this.zoneForUpdate.moistureStart,
        moistureStop: this.zoneForUpdate.moistureStop,
        waterSwitch: this.zoneForUpdate.waterSwitch,
        systemId: this.systemId
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

    updateMultipleZonesMoisture(vegetableId: number) {
      const zoneForUpdate = {
        moistureStart: this.zoneForUpdate.moistureStart,
        moistureStop: this.zoneForUpdate.moistureStop,
        waterSwitch: this.zoneForUpdate.waterSwitch,
        systemId: this.systemId
      };
      console.log(this.systemId);
      this.vegetableService.UpdateVegetablesAndZones(vegetableId, this.userId, zoneForUpdate).subscribe(
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
