import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { VegetablesForCreationModel } from 'src/app/models/VegetablesForCreationModel';

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

  AddVegetable(userId: number, vegetable: VegetablesForCreationModel): Observable<any> {
    return this.http.post(`${environment.url}/${userId}/vegetables`, vegetable);
  }

  DeleteVegetable(id: number): Observable<any> {
    return this.http.delete(`${environment.url}/vegetables/${id}`);
  }

}
