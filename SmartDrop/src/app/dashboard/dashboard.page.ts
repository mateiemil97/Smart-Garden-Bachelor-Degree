import { Component, OnInit } from '@angular/core';
import { Measurement } from '../models/measurement.model';
import { DashboardService } from './service/dashboard.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: 'dashboard.page.html',
  styleUrls: ['dashboard.page.scss']
})
export class DashboardPage implements OnInit {

  currentTemperature: Measurement;
  currentMoisture: Measurement;

  constructor(
    public dashboardService: DashboardService
  ) {}

  ngOnInit() {
    this.dashboardService.GetLatestMeasurementValue(1012, 'temperature')
      .subscribe(temp => {
        console.log(temp);
        this.currentTemperature = temp;
      });
    this.dashboardService.GetLatestMeasurementValue(1012, 'moisture')
      .subscribe(mois => {
        console.log(mois);
        this.currentMoisture = mois;
      });
  }

}