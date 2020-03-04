import { Component, OnInit } from '@angular/core';
import { UserForLogin } from 'src/app/models/userForLogin';
import { LoginService } from './login.service';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Storage } from '@ionic/storage';
import * as jwt_decode from 'jwt-decode';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
})
export class LoginPage implements OnInit {

  userForLogin: UserForLogin = new UserForLogin();
  errMessage: string;

  constructor(
    private loginService: LoginService,
    private storage: Storage,
    private router: Router
  ) { }

  ngOnInit() {
  }

  login(form: NgForm) {
    const user: UserForLogin = {
      email : form.controls['email'].value,
      password : form.controls['password'].value
    };

    this.loginService.login(user).subscribe(
      (res: any) => {
        this.storage.set('token', res.token);
        const userId = (this.getDecodedAccessToken(res.token)).userId;
        this.storage.set('userId', userId);

        this.router.navigate(['/tabs/dashboard']);
        console.log('login');
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
