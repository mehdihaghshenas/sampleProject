import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './service/auth-guard/auth-guard.service';
import { NoAuthGuard } from './service/auth-guard/no-auth-guard.service';


const routes: Routes = [
  {
    path: 'auth',
    loadChildren: () =>
      import('./fomrs/auth/auth.module').then(m => m.AuthModule),
    canActivate: [NoAuthGuard],
  },
  {
    path: '',
    loadChildren: () =>
      import('./fomrs/company/company.module').then(m => m.CompanyModule),
  },
  {
    path: '**', redirectTo: '', pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
