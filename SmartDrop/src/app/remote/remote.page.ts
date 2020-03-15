import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../dashboard/service/dashboard.service';
import { AlertController, ToastController } from '@ionic/angular';
import { CurrentState } from '../models/currentState';
import { ChangeIrigationState } from '../models/changeIrigationState';
import { IrrigationSystem } from '../models/irrigationSystem';
import {Storage} from '@ionic/storage';
@Component({
  selector: 'app-remote',
  templateUrl: 'remote.page.html',
  styleUrls: ['remote.page.scss']
})
export class RemotePage implements OnInit {

  currentState: CurrentState;
  changeIrigationState: ChangeIrigationState = new ChangeIrigationState();
  currentIrrigationSystem: IrrigationSystem;
  currentIrrigationSystemId;

  constructor(
    private dashboardService: DashboardService,
    private alertController: AlertController,
    public toastController: ToastController,
    private storage: Storage,
  ) {}

  ngOnInit() {
    this.storage.get('irrigationSystemId').then(item => {
      this.currentIrrigationSystemId = item;
      this.getCurrentState(item);
    });
  }

  ionViewWillEnter() {

  }

  getCurrentState(systemId: number) {
    this.dashboardService.GetSystemState(systemId).subscribe(state => {
      this.currentState = state;
      console.log(this.currentState);
    });
  }

  async presentIrrigationStateUpdateAlertConfirm() {
    const alert = await this.alertController.create({
      header: 'Confirm!',
      message: this.currentState.working ? 'Turn off irrigation?' : 'Turn on irrigation?',
      buttons: [
        {
          text: 'Cancel',
          role: 'cancel',
          cssClass: 'secondary',
        }, {
          text: 'Okay',
          handler: () => {

            if (this.currentState.working === true) {
              this.changeIrigationState.working = false;
              this.changeIrigationState.manual = false;
            } else if (this.currentState.working === false) {
              this.changeIrigationState.working = true;
              this.changeIrigationState.manual = true;
            }
            this.updateIrigationState(this.changeIrigationState, this.currentIrrigationSystemId);
          }
        }
      ]
    });
    await alert.present();
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

  updateIrigationState(irrigationState: ChangeIrigationState, systemId: number) {
    this.dashboardService.ChangeIrigationState(systemId, irrigationState).subscribe(
      x => console.log('Observer got a next value: ' + x),
      err => this.presentToast('An error occured. Try again later'),
      () => {
        this.presentToast(this.currentState.working ? 'Succefully turned off irrigation' : 'Succefully turned on irrigation');
        this.getCurrentState(systemId);
      });
  }

  doRefresh(refresher) {
    console.log('Begin async operation', refresher);

    setTimeout(() => {
      console.log('Async operation has ended');
      this.getCurrentState(this.currentIrrigationSystemId);
      refresher.target.complete();
    }, 2000);
  }


}
