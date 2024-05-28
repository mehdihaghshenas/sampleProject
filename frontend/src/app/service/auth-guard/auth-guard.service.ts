import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { AuthService } from 'src/app/fomrs/auth/Application/services/auth.service';
@Injectable({
  providedIn: 'root'
})
export class AuthGuard  {
  constructor(
    private router: Router,
    private authService: AuthService,
  ) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot,
  ): Observable<boolean> {
   
    return this.authService.isAuthenticated.pipe(
      take(1),
      map(resp => {
        if (!resp) {
          this.router.navigate(['/auth/login']);
        }
        return resp;
      }),
    );
  }
}
