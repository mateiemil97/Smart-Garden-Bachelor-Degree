import { Component, OnInit } from '@angular/core';
import {Storage} from '@ionic/storage';
import { IrrigationSystem } from '../models/irrigationSystem';
import { DashboardService } from '../dashboard/service/dashboard.service';
import { LoginService } from '../core/authentication/login/login.service';
import { Router } from '@angular/router';
@Component({
  selector: 'app-myaccount',
  templateUrl: './myaccount.page.html',
  styleUrls: ['./myaccount.page.scss'],
})
export class MyaccountPage implements OnInit {


  irrigationSystems: IrrigationSystem[] = [];

  constructor(
    private  storage: Storage,
    private dashboardService: DashboardService,
    private loginService: LoginService,
    private route: Router
  ) { }

  ngOnInit() {
    this.storage.get('userId').then((id) => {
      this.dashboardService.GetSystemsByUser(id).subscribe(system => {
        this.irrigationSystems = system;
        console.log(' dxsdfsdfsd' + system);
      });
    });
    console.log(this.irrigationSystems);
  }

  logout() {
    this.loginService.logout();
    this.route.navigate(['/login']);
  }

}
