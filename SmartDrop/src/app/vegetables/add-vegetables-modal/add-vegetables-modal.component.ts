import { Component, OnInit } from '@angular/core';
import { ModalController } from '@ionic/angular';
import { VegetablesServiceService } from '../services/vegetables-service.service';
import { VegetablesForCreationModel } from 'src/app/models/VegetablesForCreationModel';
import { Form, NgForm } from '@angular/forms';
import {Storage} from '@ionic/storage';
@Component({
  selector: 'app-add-vegetables-modal',
  templateUrl: './add-vegetables-modal.component.html',
  styleUrls: ['./add-vegetables-modal.component.scss'],
})
export class AddVegetablesModalComponent implements OnInit {

  public vegetable: VegetablesForCreationModel = new VegetablesForCreationModel();
  userId: number;
  constructor(
    public vegetablesService: VegetablesServiceService,
    public storage: Storage,
    public modal: ModalController
  ) { }

  ngOnInit() {
    this.storage.get('userId').then(id => this.userId = id);
  }

  createVegetable() {
    this.vegetable.userId = this.userId;
    this.vegetablesService.AddVegetable(this.userId, this.vegetable).subscribe();

  }
  dismissModal() {
    this.modal.dismiss();
  }

}
