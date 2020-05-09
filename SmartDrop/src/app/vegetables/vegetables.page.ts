import { Component, OnInit } from '@angular/core';
import { ModalController } from '@ionic/angular';
import { GlobalVegetablesComponent } from '../global-vegetables/global-vegetables.component';
import { VegetablesServiceService } from './services/vegetables-service.service';
import { VegetablesModel } from '../models/VegetablesModel';
import {Storage} from '@ionic/storage';
import { VegetablesForCreationModel } from '../models/VegetablesForCreationModel';
@Component({
  selector: 'app-vegetables',
  templateUrl: './vegetables.page.html',
  styleUrls: ['./vegetables.page.scss'],
})
export class VegetablesPage implements OnInit {

  vegetables: VegetablesModel[] = [];

  userId: number;
  constructor(
    private addVegetablesModal: ModalController,
    private vegetablesService: VegetablesServiceService,
    public storage: Storage
  ) { }

  ngOnInit() {
    this.storage.get('userId').then(item => {
      this.userId = item;
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
    const modal = await this.addVegetablesModal.create({
      component: GlobalVegetablesComponent
    });
    return await modal.present();
  }

  dismissModal() {
    this.addVegetablesModal.dismiss();
  }

}
