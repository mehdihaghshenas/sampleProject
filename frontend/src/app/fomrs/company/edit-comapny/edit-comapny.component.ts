import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { CompanyService } from '../application/company.service';
import { Observable, map } from 'rxjs';
import { ComapnyOutput, CompanyInput } from '../application/company.viewmodel';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-edit-comapny',
  templateUrl: './edit-comapny.component.html',
  styleUrls: ['./edit-comapny.component.css']
})
export class EditComapnyComponent implements OnInit {

  @ViewChild('companyEditForm') companyEditForm: NgForm
  company: CompanyInput
  constructor(private cservice: CompanyService, private router: Router) {
    // solution2 if you want use router you should run it in ctor
    // this.state = (this.router.getCurrentNavigation().extras.state['item']);
  }
  state: ComapnyOutput;
  ngOnInit() {
    // solution1 if you want to use window.history.state.item you should use it ngOnInit
    this.state = window.history.state.item;
    // this.company.name = this.state.name;
    // this.company.id = this.state.id.toString();
    this.company = { id: this.state.id.toString(), name: this.state.name } as CompanyInput;
  }
  ngAfterViewChecked(): void {
    //Called after every check of the component's view. Applies to components only.
    //Add 'implements AfterViewChecked' to the class.

  }
  submitForm() {
    this.cservice.editCompany(this.companyEditForm.value).subscribe(
      {
        next: c => {
          alert("success")
        },
        error: (e) => {
          console.log(e);
        },
        complete: () => { }
      })
  }
  submitForm2() {
    this.cservice.editCompany(this.company).subscribe(
      {
        next: c => {
          alert("success")
        },
        error: (e) => {
          console.log(e);
        },
        complete: () => { }
      })
  }
}
