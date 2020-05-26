import { Injectable } from '@angular/core';
import {environment} from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChangeIrigationState } from 'src/app/models/changeIrigationState';
import { AngularFireDatabase, AngularFireList, AngularFireObject } from 'angularfire2/database';
import { IrrigationSysteConnectedFirebase } from 'src/app/models/IrrigationSysteConnectedFirebase';
import { UpdateZoneForNotConnected } from 'src/app/models/UpdateZoneForNotConnected';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  constructor(
    public http: HttpClient,
    public firebaseDb: AngularFireDatabase
  ) { }

  public GetLatestMeasurementValue(systemId: number, sensorId: number): Observable<any> {
    return this.http.get(`${environment.url}/systems/${systemId}/measurements/${sensorId}`);
  }

  public GetLatestTemperature(systemId: number) {
    return this.http.get(`${environment.url}/systems/${systemId}/measurements/temperature`);
  }

  public GetLatestHumidity(systemId: number) {
    return this.http.get(`${environment.url}/systems/${systemId}/measurements/humidity`);
  }

  public ChangeIrigationState(systemId: number, irigationState: ChangeIrigationState): Observable<any> {
    return this.http.put(`${environment.url}/systems/${systemId}/systemState`, irigationState);
  }

  public GetSystemState(systemId: number): Observable<any> {
    return this.http.get(`${environment.url}/systems/${systemId}/currentState`);
  }

  public GetSystemsByUser(userId: number): Observable<any> {
    return this.http.get(`${environment.url}/systems/users/${userId}`);
  }

  public GetLastTimeSystemSeenOnline(series: string): AngularFireObject<IrrigationSysteConnectedFirebase> {
    return this.firebaseDb.object('System_Series/' + series + '/Last_Time_Seen_Online');
  }

  public UpdateStateWhenNotConnectedSystem(systemId: number, state: UpdateZoneForNotConnected): Observable<any> {
    return this.http.put(`${environment.url}/systems/${systemId}/systemState`, state);
  }
}
