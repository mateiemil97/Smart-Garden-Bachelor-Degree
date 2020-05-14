import { Component, OnInit } from '@angular/core';
import { ModalController } from '@ionic/angular';
import { GlobalVegetablesComponent } from '../global-vegetables/global-vegetables.component';
import { VegetablesServiceService } from './services/vegetables-service.service';
import { VegetablesModel } from '../models/VegetablesModel';
import {Storage} from '@ionic/storage';
import { VegetablesForCreationModel } from '../models/VegetablesForCreationModel';
import { ZoneForUpdate } from '../models/zoneForUpdate.model';
import { EditMoistureModalComponent } from '../schedule/edit-moisture-modal/edit-moisture-modal.component';
import { Router } from '@angular/router';
@Component({
  selector: 'app-vegetables',
  templateUrl: './vegetables.page.html',
  styleUrls: ['./vegetables.page.scss'],
})
export class VegetablesPage implements OnInit {

  vegetables: VegetablesModel[] = [];

  userId: number;
  systemId: number;
  constructor(
    private modalController: ModalController,
    private vegetablesService: VegetablesServiceService,
    public storage: Storage,
    public router: Router
  ) { }

  ngOnInit() {
    this.storage.get('userId').then(item => {
      this.userId = item;
    });
    this.storage.get('irrigationSystemId').then(item => {
      this.systemId = item;
    });
  }

  ionViewWillEnter() {
    this.vegetables = [];
    this.storage.get('userId').then(item => {
      this.getVegetables(item);
    });
  }



  getVegetables(userId: number) {
    this.vegetablesService.GetVegetables(userId).subscribe(
      items => {
        this.vegetables = items;
    });
  }

  deleteVegetable(id: number) {
    return this.vegetablesService.DeleteVegetable(id).subscribe(
      x => {
        const found = this.vegetables.findIndex(element => element.id === id);
        if (found) {
          this.vegetables.splice(found, 1);
        }
      }
    );
  }

  async openAddVegetablesModal() {
    const modal = await this.modalController.create({
      component: GlobalVegetablesComponent
    });
    return await modal.present();
  }

  dismissModal() {
    this.modalController.dismiss();
  }

  async openEditModal(vegetable) {
    const modal = await this.modalController.create({
      component: EditMoistureModalComponent,
      componentProps: {
        moistureStart: vegetable.startMoisture,
        moistureStop: vegetable.stopMoisture,
        waterSwitch: vegetable.waterSwitch,
        isAllZoneEdit: true,
        systemId: this.systemId,
        userId: this.userId,
        vegetableId: vegetable.id
      }
    });
    modal.onDidDismiss().then(
    );
    return await modal.present();
  }

}
