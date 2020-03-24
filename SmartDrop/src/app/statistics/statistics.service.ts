import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
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
}
