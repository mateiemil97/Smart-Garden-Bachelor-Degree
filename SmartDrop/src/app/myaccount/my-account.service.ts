import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IrrigationSystem } from '../models/irrigationSystem';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MyAccountService {

  constructor(
    private http: HttpClient
  ) { }

  deleteFCMToken(systemId: number): Observable<any> {
    return this.http.delete(`${environment.url}/systems/${systemId}/fcmtoken`);
  }
}
