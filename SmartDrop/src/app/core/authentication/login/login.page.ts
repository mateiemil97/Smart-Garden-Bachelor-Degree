import { Component, OnInit } from '@angular/core';
import { UserForLogin } from 'src/app/models/userForLogin';
import { LoginService } from './login.service';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Storage } from '@ionic/storage';
import * as jwt_decode from 'jwt-decode';
import { FCM } from '@ionic-native/fcm/ngx';
import { IrrigationSystem } from 'src/app/models/irrigationSystem';
import { DashboardService } from 'src/app/dashboard/service/dashboard.service';
import { FCMToken } from 'src/app/models/FCMToken';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
})
export class LoginPage implements OnInit {

  userForLogin: UserForLogin = new UserForLogin();
  errMessage: string;
  userSystems: IrrigationSystem[] = [];

  constructor(
    private loginService: LoginService,
    private storage: Storage,
    private router: Router,
    private fcm: FCM,
    private dashboardService: DashboardService
  ) { }

  ngOnInit() {
  }

  login(form: NgForm) {
    const user: UserForLogin = {
      email: form.controls['email'].value,
      password: form.controls['password'].value
    };

    this.loginService.login(user).subscribe(
      (res: any) => {
        this.storage.set('token', res.token);
        const userId = (this.getDecodedAccessToken(res.token)).userId;
        this.storage.set('userId', userId);

        this.router.navigate(['/tabs/dashboard']);
        console.log('login');
        this.fcm.getToken().then(token => {
          console.log('tokenNotification:' + token);
        });
        console.log('AAAAAAA');

        this.fcm.getToken().then(token => {
          console.log(token);

          this.dashboardService.GetSystemsByUser(userId).subscribe(system => {
            this.userSystems = system;
            this.userSystems.forEach(element => {
              // tslint:disable-next-line: label-position
              const tokenForDb: FCMToken = {
                systemId: element.systemId,
                token
              };
              console.log('logddd');
              this.loginService.postToken(tokenForDb).subscribe();
            });
          });
        });

      },
      err => {
        if (err.status === 400) {
          this.errMessage = err.message;
        }
      }
    );
  }


  logout() {
    this.storage.remove('token').then(() => {
      this.router.navigate(['/login']);
    });
  }


  getDecodedAccessToken(token: string) {
    try {
      return jwt_decode(token);
    } catch (Error) {
      return null;
    }
  }

}
