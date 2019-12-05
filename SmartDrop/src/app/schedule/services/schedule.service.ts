import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Schedule } from 'src/app/models/schedule.model';
import {environment} from './../../../environments/environment';
import { Observable } from 'rxjs';
import { ZoneForCreate } from 'src/app/models/zoneForCreate.model';
import { Zone } from 'src/app/models/zone.model';
@Injectable({
  providedIn: 'root'
})
export class ScheduleService {

  constructor(
    public http: HttpClient
  ) { }

  public UpdateSchedule(systemId: number, schedule: Schedule): Observable<any> {
    return this.http.put(`${environment.url}/systems/${systemId}/schedule`, schedule);
  }

  public GetSchedule(systemId: number): Observable<any> {
    return this.http.get(`${environment.url}/systems/${systemId}/schedule`);
  }

  public GetZones(systemId: number): Observable<any> {
    return this.http.get(`${environment.url}/systems/${systemId}/zones`);
  }

  public GetAvailablePorts(systemId: number): Observable<any> {
    return this.http.get(`${environment.url}/systems/${systemId}/ports/availables`);
  }

  public AddZone(systemId: number, zone: ZoneForCreate): Observable<any> {
    return this.http.post(`${environment.url}/systems/${systemId}/zones`, zone);
  }

  public DeleteZone(systemId: number, zoneId: number): Observable<any> {
    return this.http.delete(`${environment.url}/systems/${systemId}/zones/${zoneId}`);
  }
}
