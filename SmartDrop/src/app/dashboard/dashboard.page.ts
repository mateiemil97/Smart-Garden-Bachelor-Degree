import { Component, OnInit } from '@angular/core';
import { Measurement } from '../models/measurement.model';
import { DashboardService } from './service/dashboard.service';
import { ScheduleService } from '../schedule/services/schedule.service';
import { Zone } from '../models/zone.model';
import { ChangeIrigationState } from '../models/changeIrigationState';
import { AlertController, ToastController } from '@ionic/angular';
import { CurrentState } from '../models/currentState';
import { IrrigationSystem } from '../models/irrigationSystem';
import { LoginService } from '../core/authentication/login/login.service';
import { Router } from '@angular/router';
import { Storage } from '@ionic/storage';
import { MeasurementForDashboard } from '../models/MeasurementForDashboard';
@Component({
  selector: 'app-dashboard',
  templateUrl: 'dashboard.page.html',
  styleUrls: ['dashboard.page.scss']
})
export class DashboardPage implements OnInit {

  currentTemperature: Measurement;
  currentMoisture: MeasurementForDashboard[] = [];
  zones: Zone[] = [];
  currentState: CurrentState = new CurrentState();
  changeIrigationState: ChangeIrigationState = new ChangeIrigationState();
  irrigationSystems: IrrigationSystem[] = [];

  currentIrrigationSystem: IrrigationSystem;
  zone: MeasurementForDashboard = new MeasurementForDashboard();
  userId: number;

  constructor(
    public dashboardService: DashboardService,
    public scheduleService: ScheduleService,
    public toastController: ToastController,
    public loginService: LoginService,
    public route: Router,
    public storage: Storage,
    private alertController: AlertController,

  ) { }

  ngOnInit() {
    this.currentMoisture = [];
    this.storage.get('userId').then((id) => {
      this.userId = id;
      this.dashboardService.GetSystemsByUser(this.userId).subscribe(system => {
        this.irrigationSystems = system;
        console.log(this.irrigationSystems[0].systemId);
        this.currentIrrigationSystem = this.irrigationSystems[0];
        this.storage.set('irrigationSystemId', this.currentIrrigationSystem.systemId);
        this.getCurrentState(this.irrigationSystems[0].systemId);
      });
    });
  }

  ionViewWillEnter() {
    this.storage.get('userId').then((id) => {
      this.getCurrentState(this.currentIrrigationSystem.systemId);
      this.currentMoisture = [];
      this.getTemperature(this.currentIrrigationSystem.systemId);
    });
  }

  getTemperature(systemId: number) {
    this.dashboardService.GetLatestTemperature(this.currentIrrigationSystem.systemId).subscribe((temp: Measurement) => {
      this.currentTemperature = temp;
    });
  }

  getZones(systemIds: number) {
    this.scheduleService.GetZones(this.currentIrrigationSystem.systemId).subscribe(
      zones => {
        this.zones = zones;
        zones.forEach(element => {
          console.log(element);
          this.dashboardService.GetLatestMeasurementValue(this.currentIrrigationSystem.systemId, element.sensorId).subscribe(moist => {

            if (moist == null) {
              moist = new MeasurementForDashboard();
              moist.vegetableName = element.userVegetableName;
              moist.zone = element.name;
            }
            if (moist != null) {
              this.zone = moist;
              this.zone.vegetableName = element.userVegetableName;
            }
            this.currentMoisture.push(this.zone);
            console.log('dsdfsd');
          });
        });
      }
    );
  }

  selectBoard(systemId: number) {
    this.storage.set('irrigationSystemId', systemId);
    this.currentMoisture = [];
    this.getTemperature(systemId);
    this.getZones(systemId);
    this.getCurrentState(systemId);
  }

  // for dashboard
  getCurrentState(systemId: number) {
    this.dashboardService.GetSystemState(systemId).subscribe(state => {
      this.currentState = state;
    });
  }

  refresh() {
    this.currentMoisture = [];
    this.getZones(this.currentIrrigationSystem.systemId);
    this.getTemperature(this.currentIrrigationSystem.systemId);
    this.getCurrentState(this.currentIrrigationSystem.systemId);
  }

  doRefresh(refresher) {
    console.log('Begin async operation', refresher);

    setTimeout(() => {
      // console.log('Async operation has ended');
      this.refresh();
      refresher.target.complete();
    }, 2000);
  }

  async presentIrrigationStateUpdateAlertConfirm() {
    const alert = await this.alertController.create({
      header: 'Confirma',
      message: this.currentState.working ? 'Opreste irigarea?' : 'Pornire irigarea manuala?',
      buttons: [
        {
          text: 'Anuleaza',
          role: 'cancel',
          cssClass: 'secondary',
        }, {
          text: 'Da',
          handler: () => {

            if (this.currentState.working === true) {
              this.changeIrigationState.working = false;
              this.changeIrigationState.manual = false;
              this.changeIrigationState.automationMode = false;
            } else if (this.currentState.working === false) {
              this.changeIrigationState.working = true;
              this.changeIrigationState.manual = true;
            }
            this.updateIrigationState(
              this.changeIrigationState, this.currentIrrigationSystem.systemId,
              'Irigarea a fost oprita cu success', 'Irigarea a fost pornita cu succes', 1);
          }
        }
      ]
    });
    await alert.present();
  }

  async presentIrrigationAutomatedStateUpdateAlertConfirm() {
    const alert = await this.alertController.create({
      header: 'Confirm!',
      subHeader: this.currentState.automationMode ?
        'Daca dezactivati irigarea automata aceasta nu va mai porni la program pana la reactivare!' :
        'Daca activati irigarea, aceasta va porni in functie de programul ales'
      ,
      message: this.currentState.automationMode ? 'Dezactivati irigarea automata?' : 'Activati irigarea automata?',
      buttons: [
        {
          text: 'Anuleaza',
          role: 'cancel',
          cssClass: 'secondary',
        }, {
          text: 'Da',
          handler: () => {

            if (this.currentState.automationMode === true) {
              this.changeIrigationState.automationMode = false;
            } else if (this.currentState.automationMode === false) {
              this.changeIrigationState.automationMode = true;
              if (this.currentState.working) {
                this.changeIrigationState.manual = false;
              }

            }

            // tslint:disable-next-line: max-line-length
            this.updateIrigationState(this.changeIrigationState, this.currentIrrigationSystem.systemId, 'Irigarea automata dezactivata cu success', 'Irigarea automata activata cu succes', 2);
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
          text: 'Inchide',
          role: 'cancel',
        }
      ]
    }
    );
    toast.present();
  }

  updateIrigationState(irrigationState: ChangeIrigationState, systemId: number, stopIrr: string, startIrr: string, choose: number) {
    this.dashboardService.ChangeIrigationState(systemId, irrigationState).subscribe(
      x => console.log('Observer got a next value: ' + x),
      err => this.presentToast('A aparut o eroare. Incercati din nou'),
      () => {
        if (choose === 1) {
          this.presentToast(this.currentState.working ? stopIrr : startIrr);
        } else if (choose === 2) {
          this.presentToast(this.currentState.automationMode ? stopIrr : startIrr);
        }
        console.log("Id:" + systemId);
        this.getCurrentState(systemId);

        this.dashboardService.GetSystemState(systemId).subscribe(state => {
          console.log(state);
          if (state.working) {
            this.refresh();
          }
        });

      });
  }

}
