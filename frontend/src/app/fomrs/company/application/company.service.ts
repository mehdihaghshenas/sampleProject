import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ComapnyOutput, CompanyInput, FilterModel, FilterRes } from './company.viewmodel';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {

  constructor(private http: HttpClient) { }

  getAllCompanes(model: FilterModel): Observable<FilterRes<ComapnyOutput>> {
    return this.http.post<FilterRes<ComapnyOutput>>(environment.baseUrl + "/api/SaleCompany/GetAllCompany", model)

  }

  insertCompany(model: CompanyInput):Observable<ComapnyOutput> {
    return this.http.post<ComapnyOutput>(environment.baseUrl + "/api/SaleCompany", model)
  }
  editCompany(model: CompanyInput):Observable<ComapnyOutput> {
    return this.http.put<ComapnyOutput>(environment.baseUrl + "/api/SaleCompany", model)
  }

  createTestData() {
    this.http.post(environment.baseUrl + "/api/SampleData/Create", null).subscribe(x=>{})
  }
}





