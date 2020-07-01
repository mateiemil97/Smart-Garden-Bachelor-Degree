import { Component, OnInit } from '@angular/core';
import { ModalController, ToastController } from '@ionic/angular';
import { IrrigationSystemForCreation } from 'src/app/models/irrigationSystemForCreation';
import { NgForm } from '@angular/forms';
import {Storage} from '@ionic/storage';
import { MyAccountService } from '../my-account.service';
@Component({
  selector: 'app-add-irrigation-system-modal',
  templateUrl: './add-irrigation-system-modal.component.html',
  styleUrls: ['./add-irrigation-system-modal.component.scss'],
})
export class AddIrrigationSystemModalComponent implements OnInit {

  irrigationSystem: IrrigationSystemForCreation = new IrrigationSystemForCreation();
  userId: number;

  constructor(
    private modalController: ModalController,
    private storage: Storage,
    private myAccoutService: MyAccountService,
    private toastController: ToastController
  ) { }

  ngOnInit() {
    this.storage.get('userId').then(user => {
      this.userId = user;
    });
   }

  async dismissModal() {
    this.modalController.dismiss({
      dismissed: true
    });
  }

  addIrrigationSystem(system: NgForm) {
    const sys: IrrigationSystemForCreation = {
      name: system.value.name,
      userId: this.userId,
      seriesKey: system.value.series
    };
    this.myAccoutService.addIrrigationSystem(sys).subscribe(
      complete => {
        this.irrigationSystem.name = ' ';
        this.irrigationSystem.seriesKey = ' ';
      },
      (err: Response)  => {
        if (err.status === 404) {
          this.presentToast('Serie cheie negasita. Verificati si incercati din nou');
        } else if (err.status === 400) {
          this.presentToast('Seria cheie a fost deja folosita');
        }
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

}
