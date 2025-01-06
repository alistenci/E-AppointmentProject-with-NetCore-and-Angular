import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { LayoutsComponent } from './components/layouts/layouts.component';
import { HomeComponent } from './components/home/home.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { AuthService } from './services/auth.service';
import { inject } from '@angular/core';
import { DoctorsComponent } from './components/doctors/doctors.component';
import { PatientComponent } from './components/patient/patient.component';
import { UsersComponent } from './components/users/users.component';
import { authGuard } from './auth.guard';

export const routes: Routes = [
    {
        path: "login",
        component: LoginComponent
    },
    {
        path: "",
        component: LayoutsComponent,
        canActivateChild: [()=> inject(AuthService).isAuth()], // children router çalışmadan önce kontrol yapıyoruz. Eğer isAuth metodu true dönmüşse kullanıcı children route'a erişebilir. Eğer false ise login sayfasına yönlendirilir.
        children:[
            {
                path: "",
                component: HomeComponent
            },
            {
                path: "doctors",
                component: DoctorsComponent
            },
            {
                path: "patients",
                component: PatientComponent
            },
            {
                path: "users",
                canActivate: [authGuard],
                component: UsersComponent
            }
        ]
    },
    {
        path: "**",
        component: NotFoundComponent
    }

];
