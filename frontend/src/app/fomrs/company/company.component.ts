import { Component, OnInit } from '@angular/core';
import { AuthService } from '../auth/Application/services/auth.service';
import { CompanyService } from './application/company.service';

@Component({
  selector: 'app-company',
  templateUrl: './company.component.html',
  styleUrls: ['./company.component.css']
})
export class CompanyComponent implements OnInit {

  constructor(private authservice: AuthService, private companyService: CompanyService) { }
  isLogin = false
  ngOnInit() {
    this.isLogin = this.authservice.isLoggedIn()
  }

  logout() {
    this.authservice.signOut()
  }

  createSampleData() {
    this.companyService.createTestData()
  }

}
