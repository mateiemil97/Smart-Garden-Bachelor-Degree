import { HttpInterceptor, HttpEvent, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HttpHandler } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Storage } from '@ionic/storage';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
@Injectable()

export class Interceptor implements HttpInterceptor {

    constructor(
        private storage: Storage,
        private route: Router
    ) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return new Observable(() => {
            this.storage.get('token').then(token => {
                if (token !== null) {
                    const cloneReq = req.clone({
                        headers: req.headers.set('Authorization', 'Bearer' + token)
                    });
                    return next.handle(cloneReq).pipe(
                        tap(
                            succ => { },
                            err => {
                                if (err.status === 401) {
                                    this.storage.clear().then();
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
