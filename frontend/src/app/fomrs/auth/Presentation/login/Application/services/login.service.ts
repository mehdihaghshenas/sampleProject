import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { loginInput, loginOutput } from "../view-models/loginInputViewModel";

@Injectable(
  {
    providedIn: 'root'
  }
)
export class LoginService {
  constructor(private http: HttpClient) {
  }
  login(model: loginInput) :Observable<loginOutput>{
    return this.http.post<loginOutput>(environment.baseUrl+"/api/auth/Login",model)
  }
}
