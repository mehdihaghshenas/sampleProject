import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, from, catchError, of, finalize, lastValueFrom } from "rxjs";
import { AuthService } from "src/app/fomrs/auth/Application/services/auth.service";



@Injectable({
  providedIn:'root'
})
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) {
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return from(this.handleAccess(req, next));
  }

  private async handleAccess(req: HttpRequest<any>, next: HttpHandler):
    Promise<HttpEvent<any>> {
    const authToken = this.authService.getAuthorizationHeaderValue();
    let timeZone = null;
    timeZone = new window.Intl.DateTimeFormat().resolvedOptions().timeZone;

    req = req.clone({ setHeaders: { timezone: timeZone, Authorization: 'Bearer ' + authToken} });

    //wait 
    var res = next.handle(req)
      .pipe(
        catchError((error: any) => {
          if (error && error instanceof HttpErrorResponse) {
            if (error.status === 401) {
              this.authService.signOut();
            }
            throw new HttpErrorResponse({
              error: error.error,
              status: error.status,
              statusText: error.statusText,
              url: error.url
            });
          }
          throw of(error);
        }));
    return lastValueFrom(res)
  }

}


