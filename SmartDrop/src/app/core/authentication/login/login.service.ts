import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserForCreation } from 'src/app/models/userForCreation';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserForLogin } from 'src/app/models/userForLogin';
import { Storage } from '@ionic/storage';
@Injectable({
  providedIn: 'root'
})
export class LoginService {

  constructor(
    private http: HttpClient,
    private storage: Storage
  ) { }

  login(user: UserForLogin): Observable<any> {
    return this.http.post(environment.url + '/users/login', user);
  }

  logout() {
    this.storage.clear().then(() => {
      console.log('cleared');
    });
  }
}

