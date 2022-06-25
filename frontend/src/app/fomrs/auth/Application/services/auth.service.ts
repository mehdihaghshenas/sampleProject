import { Injectable, Input } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { BehaviorSubject } from 'rxjs';
import { LoginStatus } from 'src/app/fomrs/auth/Application/view-models/LoginStatus';
import { ShortcutService } from 'src/app/service/shortcut.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  jwtHelper = new JwtHelperService();
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated = this.isAuthenticatedSubject.asObservable();

  constructor(private router: Router, private shservice: ShortcutService) {
    this.initLoginStatus();
  }

  initLoginStatus() {
    var token = this.getAuthorizationHeaderValue();
    if (token == '') {
      this.isAuthenticatedSubject.next(false);
    }
    else {
      this.isAuthenticatedSubject.next(true);
    }
  }

  decodedToken() {
    try {
      return this.jwtHelper.decodeToken(this.getAuthorizationHeaderValue());
    } catch (error) {
      this.signOut();
      return null;
    }
  }
  isLoggedIn(): boolean|null {
    try {
      return !this.jwtHelper.isTokenExpired(this.getAuthorizationHeaderValue());
    } catch (error) {
      this.signOut();
      return null;
    }
  }

  get loginStatus(): LoginStatus {
    if (this.isLoggedIn()) {
      return LoginStatus.login;
    } else {
      if (this.shservice.checkcookie('token') == true) {
        return LoginStatus.expireToken;
      }
      return LoginStatus.logout;
    }
  }

  getAuthorizationHeaderValue(): string {
    let token = "";
    if (!this.shservice.checkcookie('token')) {
      this.isAuthenticatedSubject.next(false);
    }
    else {
      this.isAuthenticatedSubject.next(true);
      token = this.shservice.getcookie('token');
    }
    return token ?? "";
  }

  getUsername(): string {
    const tokenModel = this.decodedToken();
    if (tokenModel && tokenModel !== '') {
      for (const [key, value] of Object.entries(tokenModel)) {
        if (key.endsWith('name')) {
          return (value as any).toString();
        }
      }
    }
    return '';
  }

  setAuthorizationHeaderValue(token: string): void {
    this.shservice.SetCookie('token', token, 99999);
    this.isAuthenticatedSubject.next(true);
  }


  signOut(): void {
    setTimeout(() => {
      this.shservice.DeleteCookie('token');
      this.isAuthenticatedSubject.next(false);
        this
          .router
          .navigateByUrl('/auth/login');
    }, 1);
  }
}
