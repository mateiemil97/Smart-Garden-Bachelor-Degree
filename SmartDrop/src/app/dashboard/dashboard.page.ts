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
  irrigationSystems: IrrigationSystem[] = [];

  currentIrrigationSystem: IrrigationSystem;

  userId: number;

  constructor(
    public dashboardService: DashboardService,
    public scheduleService: ScheduleService,
    public toastController: ToastController,
    public loginService: LoginService,
    public route: Router,
    public storage: Storage
  ) { }

  ngOnInit() {
    this.storage.get('userId').then((id) => {
      this.userId = id;
      this.dashboardService.GetSystemsByUser(this.userId).subscribe(system => {
        this.irrigationSystems = system;
        console.log(this.irrigationSystems);
        this.currentIrrigationSystem = this.irrigationSystems[0];
        this.storage.set('irrigationSystemId', this.currentIrrigationSystem.systemId);
      });
    });
  }

  ionViewWillEnter() {
     this.currentMoisture = [];
     this.getZones(this.currentIrrigationSystem.systemId);
     this.getTemperature(this.currentIrrigationSystem.systemId);
  }

  getTemperature(systemId: number) {
    this.dashboardService.GetLatestTemperature(this.currentIrrigationSystem.systemId).subscribe((temp: Measurement) => {
      this.currentTemperature = temp;
      console.log(this.currentTemperature);
    });
  }

  getZones(systemIds: number) {
    this.scheduleService.GetZones(this.currentIrrigationSystem.systemId).subscribe(
      zones => {
        zones.forEach(element => {
          this.dashboardService.GetLatestMeasurementValue(this.currentIrrigationSystem.systemId, element.sensorId).subscribe(moist => {
            this.currentMoisture.push(moist);
            console.log(this.currentMoisture);
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
      console.log(this.currentState);
    });
  }

}
