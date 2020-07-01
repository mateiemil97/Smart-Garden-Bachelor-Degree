import { Component, OnInit } from '@angular/core';
import { UserForCreation } from '../../../models/userForCreation';
import { NgForm, FormGroup } from '@angular/forms';
import { SignupService } from './signup.service';
import { ToastController } from '@ionic/angular';
import { HttpResponse } from '@angular/common/http';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.page.html',
  styleUrls: ['./signup.page.scss'],
})
export class SignupPage implements OnInit {

  userForRegister: UserForCreation = new UserForCreation();

  emailPattern = '^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$';

  constructor(
    public signupService: SignupService,
    public toastController: ToastController,

  ) { }

  ngOnInit() {
  }

  register(form: NgForm) {

    const user: UserForCreation = {
      firstName: form.controls['firstName'].value,
      lastName: form.controls['lastName'].value,
      email: form.controls['email'].value,
      password: form.controls['password'].value,
      confirmPassword: form.controls['confirmPassword'].value,
      country: form.controls['country'].value,
      city: form.controls['city'].value,

    };
    this.signupService.signup(user).subscribe(
      (res: any) => {
        if (res.succeeded) {
          this.resetForm();
          this.presentToast('Contul a fost creat!');
        } else {
          res.errors.forEach(element => {
            switch (element.code) {
              case 'DuplicateUserName':
                this.presentToast('Username deja existent.');
                break;

              default:
                this.presentToast(`${element.code}.Inregistrarea a esuat!`);
                break;
            }
          });
        }
      }
    );
  }

  async presentToast(message: string) {
    const toast = await this.toastController.create({
      message,
      duration: 2000,
      buttons: [
        {
          text: 'Close',
          role: 'cancel',
        }
      ]
    }
    );
    toast.present();
  }

  resetForm() {
    this.userForRegister.city = '';
    this.userForRegister.confirmPassword = '';
    this.userForRegister.country = '';
    this.userForRegister.email = '';
    this.userForRegister.firstName = '';
    this.userForRegister.lastName = '';
    this.userForRegister.password = '';
  }

}
