import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IrrigationSystemForCreation } from '../models/irrigationSystemForCreation';

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

  addIrrigationSystem(addIrrigationSystem: IrrigationSystemForCreation): Observable<any> {
    return this.http.post(`${environment.url}/systems/system/users`, addIrrigationSystem);
  }
}
