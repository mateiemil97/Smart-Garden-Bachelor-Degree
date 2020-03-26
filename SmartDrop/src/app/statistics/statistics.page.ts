import { Component, OnInit } from '@angular/core';
import { ChartDataSets } from 'chart.js';
import { Label, Color } from 'ng2-charts';
import { HttpClient } from '@angular/common/http';
import { WeekDay } from '@angular/common';
import { SensorsForStatistics } from '../models/SensorsForStatistics';
import { StatisticsService } from './statistics.service';
import { Storage } from '@ionic/storage';
import { MeasurementsForGraphic } from '../models/measurementsForGraphic';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.page.html',
  styleUrls: ['./statistics.page.scss'],
})
export class StatisticsPage implements OnInit {

  sensors: SensorsForStatistics[] = [];
  currentSensor: SensorsForStatistics;
  dayMonth: string;
  values: MeasurementsForGraphic[] = [];
  systemId: number;
  date: string;
  constructor(
    private http: HttpClient,
    private statisticsService: StatisticsService,
    private storage: Storage
  ) {
  }

  ngOnInit() {
    this.storage.get('irrigationSystemId').then(id => {
      this.systemId = id;
      this.statisticsService.GetSensors(id).subscribe(item => {
        this.sensors = item;
        console.log(this.sensors);
      });
    });
    this.dayMonth = 'day';

  }

  // tslint:disable-next-line: member-ordering
  chartData: ChartDataSets[] = [{ data: [], label: '' }];
  // tslint:disable-next-line: member-ordering
  chartLabels: Label[];
  // Options
  // tslint:disable-next-line: member-ordering
  chartOptionsMoisture = {
    scales: {
      yAxes: [{
        ticks: {
          max: 100,
          min: 0,
          stepSize: 10
        }
      }],
      xAxes: [{
        type: 'time'
      }]
    },
    responsive: true,
    maintainAspectRatio: false,
    title: {
      display: true,
      text: 'Istoric umiditate sol'
    },
    pan: {
      // enabled: true,
      mode: 'xy'
    },
    zoom: {
      enabled: false,
      mode: 'xy'
    },
  };

  //chart options for temperature

  // tslint:disable-next-line: member-ordering
  chartOptionsTemperature = {
    scales: {
      yAxes: [{
        ticks: {
          max: 60,
          min: -5,
          stepSize: 5
        }
      }],
      xAxes: [{
        type: 'time'
      }]
    },
    responsive: true,
    title: {
      display: true,
      text: 'Istoric temperatura in °C'
    },
    maintainAspectRatio: false,
    pan: {
      // enabled: true,
      mode: 'xy'
    },
    zoom: {
      enabled: false,
      mode: 'xy'
    },
  };


  // tslint:disable-next-line: member-ordering
  chartColors: Color[] = [
    {
      borderColor: '#79ae39',
      //backgroundColor: '#79ae39'
    }
  ];
  // tslint:disable-next-line: member-ordering
  chartType = 'line';



  public getData() {
    const dateSplited = this.date.split('T');
    this.statisticsService.GetStatisticData(this.systemId, this.currentSensor.sensorId, this.dayMonth, dateSplited[0]).subscribe(res => {
      this.values = res;
      this.chartLabels = [];
      this.chartData[0].data = [];
      console.log(this.values);

      for (const entry of this.values) {
        if (this.dayMonth === 'month') {
          const val = entry.dateTime.split('T');
          this.chartLabels.push(val[0]);
          this.chartData[0].data.push(entry.value);
        } else {
          this.chartLabels.push(entry.dateTime);
          this.chartData[0].data.push(entry.value);
        }
      }
    });
  }

  typeChanged(e) {
    this.chartData[0].data = [];
    console.log(this.values);
    this.getData();
    if (e === 'month') {
      this.chartType = 'bar';
      this.chartColors = [{ backgroundColor: '#79ae39' }];
    } else {
      this.chartType = 'line';
      this.chartColors = [{ backgroundColor: '' }];
    }
    console.log(this.chartType);
  }
}
