import { Component, OnInit } from '@angular/core';
import { Measurement } from '../models/measurement.model';
import { DashboardService } from './service/dashboard.service';
import { ScheduleService } from '../schedule/services/schedule.service';
import { Zone } from '../models/zone.model';
import { environment } from '../../environments/environment';
import { mergeMap, delay } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard',
  templateUrl: 'dashboard.page.html',
  styleUrls: ['dashboard.page.scss']
})
export class DashboardPage implements OnInit {

  currentTemperature: Measurement;
  currentMoisture: Measurement[] = [];
  zones: Zone[] = [];
  currentState = false;

  constructor(
    public dashboardService: DashboardService,
    public scheduleService: ScheduleService
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
  }
  changeSystemState() {
    this.currentState = !this.currentState;
  }
}
