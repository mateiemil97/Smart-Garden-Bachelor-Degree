import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, CanActivate, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { Storage } from '@ionic/storage';
@Injectable({
  providedIn: 'root'
})

export class AuthGuard implements CanActivate {

  token: string;

  constructor(
    private storage: Storage,
    private route: Router
  ) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot):
    boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {

    return new Promise((resolve) => {
      this.storage.get('token').then((token) => {
        console.log(token);
        if (token !== null) {
          console.log(token);
          resolve(true);
        } else {
          this.route.navigate(['/login']);
          resolve(false);
        }
      });
    });
  }
}
