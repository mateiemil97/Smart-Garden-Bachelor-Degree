import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Schedule } from 'src/app/models/schedule.model';
import {environment} from './../../../environments/environment';
import { Observable } from 'rxjs';
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
}
