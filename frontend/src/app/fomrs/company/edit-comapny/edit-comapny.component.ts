import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { CompanyService } from '../application/company.service';

@Component({
  selector: 'app-edit-comapny',
  templateUrl: './edit-comapny.component.html',
  styleUrls: ['./edit-comapny.component.css']
})
export class EditComapnyComponent implements OnInit {

  @ViewChild('companyEditForm') companyEditForm: NgForm
  constructor(private cservice: CompanyService) { }

  ngOnInit() {
  }

  submitForm() {
    this.cservice.editCompany(this.companyEditForm.value).subscribe(c => {
      alert("success")
    },(e)=>{
      console.log(e);
    })
  }
}
