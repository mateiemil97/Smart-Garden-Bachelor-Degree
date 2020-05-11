import { Component, OnInit } from '@angular/core';
import { ModalController } from '@ionic/angular';
import { AddVegetablesModalComponent } from '../vegetables/add-vegetables-modal/add-vegetables-modal.component';
import { GlobalVegetablesService } from './services/global-vegetables.service';
import { GlobalVegetablesModel } from '../models/globalVegetablesModel';
import { VegetablesServiceService } from '../vegetables/services/vegetables-service.service';
import { VegetablesModel } from '../models/VegetablesModel';
import { VegetablesForCreationModel } from '../models/VegetablesForCreationModel';
import {Storage} from '@ionic/storage';
@Component({
  selector: 'app-global-vegetables',
  templateUrl: './global-vegetables.component.html',
  styleUrls: ['./global-vegetables.component.scss'],
})
export class GlobalVegetablesComponent implements OnInit {

  globalVegetables: GlobalVegetablesModel[] = [];
  userId: number;

  constructor(
    public modalForAddNewVegetable: ModalController,
    public globalVegetablesService: GlobalVegetablesService,
    public vegetablesService: VegetablesServiceService,
    public storage: Storage
  ) { }

  ngOnInit() {
    this.storage.get('userId').then(item => this.userId = item);
    this.getGlobalVegetables();
  }

  async openAddNewVegetable() {
    const modal = await this.modalForAddNewVegetable.create({
      component: AddVegetablesModalComponent
    });
    return await modal.present();
  }

  dismissModal() {
    this.modalForAddNewVegetable.dismiss();
  }

  getGlobalVegetables() {
    this.globalVegetablesService.GetVegetables().subscribe(items => {
      this.globalVegetables = items;
      console.log(this.globalVegetables);
    });
  }

  addVegetableToList(vegetable: VegetablesModel) {

    const vegetableForCreation: VegetablesForCreationModel = {
      name: vegetable.name,
      startMoisture: vegetable.startMoisture,
      stopMoisture: vegetable.stopMoisture,
      userId: this.userId
    };

    this.vegetablesService.AddVegetable(this.userId, vegetableForCreation).subscribe();
  }


}
