import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../application/company.service';
import { ComapnyOutput, emptyFilter } from '../application/company.viewmodel';
import { Router } from '@angular/router';

@Component({
  selector: 'app-company-list',
  templateUrl: './company-list.component.html',
  styleUrls: ['./company-list.component.css']
})
export class CompanyListComponent implements OnInit {
  sendDataToEdit(selected: ComapnyOutput) {
    this.router.navigate(['/company/edit'], { state: { 'item': selected } });
  }

  constructor(private companyService: CompanyService, private router: Router) { }

  compnayList: ComapnyOutput[] = []
  ngOnInit() {
    this.companyService.getAllCompanes(emptyFilter).subscribe(
      {
        next: c => {
          this.compnayList = c.data;
        }
      })
  }

}
