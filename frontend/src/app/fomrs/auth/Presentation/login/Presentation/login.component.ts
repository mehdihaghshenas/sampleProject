import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { AuthService } from '../../../Application/services/auth.service';
import { LoginService } from '../Application/services/login.service';

@Component({
  selector: 'pr-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {

  @ViewChild('loginForm') loginForm: NgForm

  constructor(private loginService: LoginService, private authservice: AuthService, private router:Router) { }
  async ngOnInit() {

  }
  login: Subscription
  async submitForm() {
    this.loginService.login(this.loginForm.value).subscribe(output=>{
      this.authservice.setAuthorizationHeaderValue(output.token);
      this.router.navigateByUrl("/")
    })

  }

  ngOnDestroy(): void {
    this.login?.unsubscribe();
  }
}
