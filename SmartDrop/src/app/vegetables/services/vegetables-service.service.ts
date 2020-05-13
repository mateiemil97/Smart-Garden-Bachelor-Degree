import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { VegetablesForCreationModel } from 'src/app/models/VegetablesForCreationModel';
import { ZoneForUpdate } from 'src/app/models/zoneForUpdate.model';

@Injectable({
  providedIn: 'root'
})
export class VegetablesServiceService {

  constructor(
    public http: HttpClient
  ) { }

  GetVegetables(userId: number): Observable<any> {
    return this.http.get(`${environment.url}/${userId}/vegetables`);
  }

  GetVegetable(userId: number, id: number): Observable<any> {
    return this.http.get(`${environment.url}/${userId}/vegetables/${id}`);
  }

  AddVegetable(userId: number, vegetable: VegetablesForCreationModel): Observable<any> {
    return this.http.post(`${environment.url}/${userId}/vegetables`, vegetable);
  }

  DeleteVegetable(id: number): Observable<any> {
    return this.http.delete(`${environment.url}/vegetables/${id}`);
  }

  UpdateVegetablesAndZones(vegetableId: number, userId: number, zoneForUpdate: ZoneForUpdate): Observable<any> {
    return this.http.put(`${environment.url}/${userId}/vegetables/${vegetableId}`, zoneForUpdate);
  }
}
