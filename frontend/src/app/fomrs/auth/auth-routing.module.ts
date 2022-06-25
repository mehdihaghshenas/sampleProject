import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import { AuthComponent } from './Presentation/auth.component';
import {LoginComponent} from './Presentation/login/Presentation/login.component';

const routes: Routes = [
    {
        path: '',
        component: AuthComponent,
        children: [
            {
                path: 'login',
                loadChildren: () => import ('./Presentation/login/login.module').then(c => c.LoginModule),
                component: LoginComponent
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AuthRoutingModule {}
