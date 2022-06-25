import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { CompanyService } from '../application/company.service';

@Component({
  selector: 'app-create-comapny',
  templateUrl: './create-comapny.component.html',
  styleUrls: ['./create-comapny.component.css']
})
export class CreateComapnyComponent implements OnInit {

  @ViewChild('companyForm') companyForm: NgForm
  constructor(private cservice: CompanyService) { }

  ngOnInit() {
  }

  submitForm() {
    this.cservice.insertCompany(this.companyForm.value).subscribe(c => {
      alert("success")
    },(e)=>{
      console.log(e);
    })
  }
}
