import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { SensorsForStatistics } from '../models/SensorsForStatistics';

@Injectable({
  providedIn: 'root'
})
export class StatisticsService {

  constructor(
    private http: HttpClient
  ) { }

  public GetSensors(systemId: number): Observable<any> {
    return this.http.get(`${environment.url}/systems/${systemId}/sensors/statistics`);
  }

  public GetStatisticData(systemId: number, sensorId: number, type: string, dateTime: string): Observable<any> {
    return this.http.get(`${environment.url}/systems/${systemId}/measurements/${sensorId}/statistics/${type}?date=${dateTime}`);
  }
}
