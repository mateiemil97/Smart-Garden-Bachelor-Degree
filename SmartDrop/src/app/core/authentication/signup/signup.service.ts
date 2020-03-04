import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserForCreation } from './../../../models/userForCreation';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignupService {

  constructor(
    public http: HttpClient
  ) { }

  signup(userForCreation: UserForCreation): Observable<any> {
    return this.http.post(environment.url + '/users/register', userForCreation);
  }
}
