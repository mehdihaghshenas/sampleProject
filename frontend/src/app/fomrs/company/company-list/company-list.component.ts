import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../application/company.service';
import { ComapnyOutput, emptyFilter } from '../application/company.viewmodel';

@Component({
  selector: 'app-company-list',
  templateUrl: './company-list.component.html',
  styleUrls: ['./company-list.component.css']
})
export class CompanyListComponent implements OnInit {

  constructor(private companyService: CompanyService) { }

  compnayList: ComapnyOutput[]=[]
  ngOnInit() {
    this.companyService.getAllCompanes(emptyFilter).subscribe(c=>{
      this.compnayList = c.data;
    })
  }

}
