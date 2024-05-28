import { Injectable } from '@angular/core';
import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { take, map } from 'rxjs/operators';
import { AuthService } from 'src/app/fomrs/auth/Application/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class NoAuthGuard  {
  constructor(
    private router: Router,
    private authService: AuthService,
  ) { }

  canActivate(
  ): Observable<boolean> {

    return this.authService.isAuthenticated.pipe(take(1), map(isAuth => {
      if (isAuth) {
        this.router.navigate(["/"]);
      }
      return !isAuth
    }));

  }
}
