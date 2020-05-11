import { Component, OnInit } from '@angular/core';
import { Storage } from '@ionic/storage';
import { IrrigationSystem } from '../models/irrigationSystem';
import { DashboardService } from '../dashboard/service/dashboard.service';
import { LoginService } from '../core/authentication/login/login.service';
import { Router } from '@angular/router';
import { MyAccountService } from './my-account.service';
import { ModalController } from '@ionic/angular';
import { AddIrrigationSystemModalComponent } from './add-irrigation-system-modal/add-irrigation-system-modal.component';
@Component({
  selector: 'app-myaccount',
  templateUrl: './myaccount.page.html',
  styleUrls: ['./myaccount.page.scss'],
})
export class MyaccountPage implements OnInit {


  irrigationSystems: IrrigationSystem[] = [];
  irrigationSystemsForDelete: IrrigationSystem[] = [];
  constructor(
    private storage: Storage,
    private dashboardService: DashboardService,
    private loginService: LoginService,
    private route: Router,
    private myAccoutService: MyAccountService,
    private addIrrigationModal: ModalController
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

  async openAddIrrigationSystemModal() {
    const modal = await this.addIrrigationModal.create({
      component: AddIrrigationSystemModalComponent
    });
    return await modal.present();
  }

  logout() {
    this.storage.get('userId').then((id) => {
      console.log(id);
      this.dashboardService.GetSystemsByUser(id).subscribe(system => {
        this.irrigationSystemsForDelete = system;
        this.irrigationSystemsForDelete.forEach(element => {
          console.log('log out' + element.systemId);
          this.myAccoutService.deleteFCMToken(element.systemId).subscribe();
        });
      });
      this.loginService.logout();
      this.route.navigate(['/login']);
    });

  }

  navigateToVegetables() {
    this.route.navigateByUrl('/vegetables');
  }

}
