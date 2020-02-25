import { HttpInterceptor, HttpEvent, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HttpHandler } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Storage } from '@ionic/storage';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { LoginService } from '../core/authentication/login/login.service';
@Injectable()

export class Interceptor implements HttpInterceptor {

    constructor(
        private storage: Storage,
        private route: Router,
        private loginSeervice: LoginService
    ) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return new Observable<HttpEvent<any>>(() => {
            this.storage.get('token').then(token => {
                if (token !== null) {
                    const cloneReq = req.clone({
                        headers: req.headers.set('Authorization', 'Bearer ' + token)
                    });
                    return next.handle(cloneReq).pipe(
                        tap(
                            succ => { },
                            err => {
                                if (err.status === 401) {
                                    this.loginSeervice.logout();
                                    this.route.navigate(['/login']);
                                }
                            }
                        )
                    );
                } else {
                    return next.handle(req.clone());
                }
            });
        });
    }
}
