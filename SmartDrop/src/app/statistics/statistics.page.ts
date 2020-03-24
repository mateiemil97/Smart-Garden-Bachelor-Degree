import { Component, OnInit } from '@angular/core';
import { ChartDataSets } from 'chart.js';
import { Label, Color } from 'ng2-charts';
import { HttpClient } from '@angular/common/http';
import { WeekDay } from '@angular/common';
import { SensorsForStatistics } from '../models/SensorsForStatistics';
import { StatisticsService } from './statistics.service';
import { Storage } from '@ionic/storage';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.page.html',
  styleUrls: ['./statistics.page.scss'],
})
export class StatisticsPage implements OnInit {

  sensors: SensorsForStatistics[] = [];
  currentSensor: SensorsForStatistics;
  dayMonth: string;
  constructor(
    private http: HttpClient,
    private statisticsService: StatisticsService,
    private storage: Storage
  ) { }

  
  periodType = 'day';

  chartData: ChartDataSets[] = [{ data: [], label: 'Stock price' }];
  chartLabels: Label[];

  // Options
  chartOptions = {
    scales: {
      yAxes: [{
        ticks: {
          max: 100,
          min: 0,
          stepSize: 10
        }
      }],
      xAxes: [{
        type: 'time',
        time: {
          unit: this.dayMonth === 'month' ? 'month' : 'day'
        }
      }]
    },
    responsive: true,
    title: {
      display: true,
      text: 'Historic Stock price'
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
  chartColors: Color[] = [
    {
      borderColor: '#000000',
      backgroundColor: '#ff00ff'
    }
  ];
  chartType = 'line';
  showLegend = false;
  // For search
  stock = '';

  ngOnInit() {
    this.storage.get('irrigationSystemId').then(id => {
      this.statisticsService.GetSensors(id).subscribe(item => {
        this.sensors = item;
        console.log(this.sensors);
      });
    });
    this.dayMonth = 'day';
  }


  getData() {
    this.http.get(`https://financialmodelingprep.com/api/v3/historical-price-full/${this.stock}?from=2018-03-12&to=2019-03-12`).subscribe(res => {
      const history = res['historical'];

      this.chartLabels = [];
      this.chartData[0].data = [];

      for (const entry of history) {
        this.chartLabels.push(entry.date);
        this.chartData[0].data.push(entry.close);
      }
    });
  }

  typeChanged(e) {
    const on = e.detail.checked;
    this.chartType = on ? 'line' : 'bar';
  }

}
