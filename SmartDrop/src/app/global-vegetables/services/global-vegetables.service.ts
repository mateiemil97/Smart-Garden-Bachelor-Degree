import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GlobalVegetablesService {

  constructor(
    public http: HttpClient
  ) { }

  public GetVegetables(): Observable<any> {
    return this.http.get(`${environment.url}/globalVegetables`);
  }

}
