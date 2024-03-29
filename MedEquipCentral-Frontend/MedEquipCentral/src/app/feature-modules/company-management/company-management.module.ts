import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppComponent } from '../../app.component';
import { CompanyFormComponent } from './company-form/company-form.component';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CompanyComponent } from './company/company.component';
import { CompaniesComponent } from './companies/companies.component';
import { NgbModalModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppointmentFormComponent } from './company/appointment-form/appointment-form.component';
import { EquipmentManagementModule } from "../equipment-management/equipment-management.module";
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { CompanyCalendarComponent } from './company-calendar/company-calendar.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ZXingScannerModule } from '@zxing/ngx-scanner';
import { PickupUsingQrComponent } from './pickup-using-qr/pickup-using-qr.component';
import { SharedModule } from "../../shared/shared.module";

@NgModule({
    declarations: [
        CompanyFormComponent,
        CompanyComponent,
        CompaniesComponent,
        AppointmentFormComponent,
        CompanyCalendarComponent,
        PickupUsingQrComponent,
    ],
    imports: [
        BrowserModule,
        ReactiveFormsModule,
        HttpClientModule,
        FormsModule,
        NgbModule,
        EquipmentManagementModule,
        CalendarModule.forRoot({
            provide: DateAdapter,
            useFactory: adapterFactory,
        }),
        BrowserAnimationsModule,
        NgbModalModule,
        ZXingScannerModule,
        SharedModule,
    ],
    exports: [
        CompanyComponent,
        CompanyFormComponent,
        CompaniesComponent,
        CompanyCalendarComponent,
        PickupUsingQrComponent,
    ],
    providers: [],
    bootstrap: [AppComponent]
})
export class CompanyManagementModule { }
