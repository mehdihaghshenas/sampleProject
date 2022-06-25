import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CompanyRoutes } from './company.routing';
import { CompanyListComponent } from './company-list/company-list.component';
import { CreateComapnyComponent } from './create-comapny/create-comapny.component';
import { CompanyComponent } from './company.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EditComapnyComponent } from './edit-comapny/edit-comapny.component';

@NgModule({
  imports: [
    CommonModule,
    CompanyRoutes,
    FormsModule,
    ReactiveFormsModule,
  ],
  declarations: [CompanyComponent, CompanyListComponent, CreateComapnyComponent, EditComapnyComponent]
})
export class CompanyModule { }
