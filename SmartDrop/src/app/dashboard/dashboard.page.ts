import { Component, OnInit } from '@angular/core';
import { Measurement } from '../models/measurement.model';
import { DashboardService } from './service/dashboard.service';
import { ScheduleService } from '../schedule/services/schedule.service';
import { Zone } from '../models/zone.model';
import { environment } from '../../environments/environment';
import { mergeMap, delay } from 'rxjs/operators';
import { ChangeIrigationState } from '../models/changeIrigationState';
import { AlertController, ToastController } from '@ionic/angular';
import { CurrentState } from '../models/currentState';

@Component({
  selector: 'app-dashboard',
  templateUrl: 'dashboard.page.html',
  styleUrls: ['dashboard.page.scss']
})
export class DashboardPage implements OnInit {

  currentTemperature: Measurement;
  currentMoisture: Measurement[] = [];
  zones: Zone[] = [];
  currentState: CurrentState;
  changeIrigationState: ChangeIrigationState = new ChangeIrigationState();
  constructor(
    public dashboardService: DashboardService,
    public scheduleService: ScheduleService,
    private alertController: AlertController,
    public toastController: ToastController,
  ) { }

  ngOnInit() {
  }

  ionViewWillEnter() {
    this.dashboardService.GetLatestTemperature(environment.systemId).subscribe((temp: Measurement) => {
      this.currentTemperature = temp;
    });
    this.currentMoisture = [];
    this.scheduleService.GetZones(environment.systemId).subscribe(
      zones => {
        zones.forEach(element => {
          this.dashboardService.GetLatestMeasurementValue(environment.systemId, element.sensorId).subscribe(moist => {
            this.currentMoisture.push(moist);
            console.log(this.currentMoisture);
          });
        });
      }
    );
    this.getCurrentState();
  }
  updateIrigationState(irrigationState: ChangeIrigationState) {
    this.dashboardService.ChangeIrigationState(environment.systemId, irrigationState).subscribe(
      x => console.log('Observer got a next value: ' + x),
      err => this.presentToast('An error occured. Try again later'),
      () => {
        this.presentToast(this.currentState.working ? 'Succefully turned off irrigation' : 'Succefully turned on irrigation');
        this.getCurrentState();
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
            this.updateIrigationState(this.changeIrigationState);
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

  getCurrentState() {
    this.dashboardService.GetSystemState(environment.systemId).subscribe(state => {
      this.currentState = state;
      console.log(this.currentState);
    });
  }

}
