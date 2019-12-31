import { Injectable } from '@angular/core';
import {environment} from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChangeIrigationState } from 'src/app/models/changeIrigationState';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  constructor(
    public http: HttpClient
  ) { }

  public GetLatestMeasurementValue(systemId: number, sensorId: number): Observable<any> {
    return this.http.get(`${environment.url}/systems/${systemId}/measurements/${sensorId}`);
  }

  public GetLatestTemperature(systemId: number) {
    return this.http.get(`${environment.url}/systems/${systemId}/measurements/temperature`);
  }

  public ChangeIrigationState(systemId: number, irigationState: ChangeIrigationState): Observable<any> {
    return this.http.put(`${environment.url}/systems/${systemId}/systemState`, irigationState);
  }

  public GetSystemState(systemId: number): Observable<any> {
    return this.http.get(`${environment.url}/systems/${systemId}/currentState`);
  }
}
