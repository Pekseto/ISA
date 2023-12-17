import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from '../../feature-modules/layout/home/home.component';
import { CompanyFormComponent } from '../../feature-modules/company-management/company-form/company-form.component';
import { CompanyComponent } from '../../feature-modules/company-management/company/company.component';
import { UserProfileComponent } from '../../feature-modules/user-management/user-profile/user-profile.component';
import { CompaniesComponent } from '../../feature-modules/company-management/companies/companies.component';
import { EquipmentSearchComponent } from '../../feature-modules/equipment-management/equipment-search/equipment-search.component';
import { SystemAdminsManagementComponent } from '../../feature-modules/user-management/system-admins-management/system-admins-management.component';
import {RegistrationComponent} from "../auth/registration/registration.component";
import {LoginComponent} from "../auth/login/login.component";
import {VerificationComponent} from "../auth/verification/verification.component";
import { CompanyCalendarComponent } from '../../feature-modules/company-management/company-calendar/company-calendar.component';
import { ChangePasswordFormComponent } from '../../feature-modules/user-management/change-password-form/change-password-form.component';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'company-form', component: CompanyFormComponent },
  { path: 'company/:id', component: CompanyComponent },
  { path: 'profile', component: UserProfileComponent },
  { path: 'companies', component: CompaniesComponent},
  { path: 'equipment/search', component: EquipmentSearchComponent },
  { path: 'register', component: RegistrationComponent},
  { path: 'login', component: LoginComponent},
  { path: 'verify/:userId', component: VerificationComponent},
  { path: 'system-admins-management', component: SystemAdminsManagementComponent },
  { path: 'company-calendar', component: CompanyCalendarComponent },
  { path: 'change-password', component: ChangePasswordFormComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
