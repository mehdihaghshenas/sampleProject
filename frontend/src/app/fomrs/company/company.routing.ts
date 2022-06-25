import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from 'src/app/service/auth-guard/auth-guard.service';
import { CompanyListComponent } from './company-list/company-list.component';
import { CompanyComponent } from './company.component';
import { CreateComapnyComponent } from './create-comapny/create-comapny.component';
import { EditComapnyComponent } from './edit-comapny/edit-comapny.component';

const routes: Routes = [
  {
    path: "company",
    component: CompanyComponent,
    children: [
      {
        path: 'list',
        component: CompanyListComponent
      },
      {
        path: 'create',
        component: CreateComapnyComponent,
        canActivate: [AuthGuard]
      },
      {
        path: 'edit',
        component: EditComapnyComponent,
        // Comment this to can test interceptor redirect
        // canActivate: [AuthGuard]
      },
      {
        path: '**', redirectTo: 'list', pathMatch: 'full'
      }
    ]
  },
  {
    path: '**', redirectTo: 'company', pathMatch: 'full'
  }
];

export const CompanyRoutes = RouterModule.forChild(routes);
