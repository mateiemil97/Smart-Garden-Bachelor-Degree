import { Component, OnInit } from '@angular/core';
import { UserForCreation } from '../models/userForCreation';
import { NgForm, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.page.html',
  styleUrls: ['./signup.page.scss'],
})
export class SignupPage implements OnInit {

  userForRegister: UserForCreation = new UserForCreation();

  emailPattern = '^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$';

  constructor() { }

  ngOnInit() {
  }

  register(form: NgForm) {
    if (form.valid) {
      console.log(form.controls['email']);
    }
  }

}
