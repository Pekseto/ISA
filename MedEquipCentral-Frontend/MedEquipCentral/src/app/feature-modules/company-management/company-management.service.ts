import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Company } from './model/company.model';
import { Equipment } from './model/equipment.model';
import { Appointment } from './model/appointment.model';
import { environment } from '../../shared/environment';
import { User } from '../../core/auth/model/user.model';

@Injectable({
  providedIn: 'root'
})
export class CompanyManagementService {


  private apiUrl = environment.apiHost;

  constructor(private http: HttpClient) { }

  addCompany(company: Company): Observable<Company> {
    return this.http.post<Company>(this.apiUrl + 'company/', company);
  }

  getCompany(id: number): Observable<Company> {
    return this.http.get<Company>(this.apiUrl + 'company/getById/' + id);
  }

  getCompanyAdmins(copmanyId: number, adminId: number): Observable<User[]> {
    return this.http.get<User[]>(environment.apiHost + 'user/getOtherCompanyAdmins/' + copmanyId + '/' + adminId);
  }

  updateCompany(company: Company): Observable<Company> {
    return this.http.put<Company>(this.apiUrl + 'company', company)
  }

  getAll(): Observable<any> {
    return this.http.get<any>(this.apiUrl + 'company/getAll');
  }

  getAllBySearch(dataIn: any): Observable<any> {
    return this.http.post<any>(this.apiUrl +  'company/getAllBySearch', dataIn);
  }

  removeCompanyAdmin(user: User): Observable<User> {
    return this.http.put<User>(this.apiUrl + 'user/' + user.id + '/removeAdmin', user);
  }

  getEquipment(companyId: number): Observable<Equipment[]> {
    return this.http.get<Equipment[]>(this.apiUrl + 'equipment/getAllForCompany/' + companyId);
  }

  createAppointment(appointment: Appointment): Observable<Appointment> {
    return this.http.post<Appointment>(this.apiUrl + 'appointment/createAppointment', appointment)
  }

  updateAppointment(appointment: Appointment): Observable<Appointment> {
    return this.http.put<Appointment>(this.apiUrl + 'appointment/update', appointment)
  }

  cancelAppointment(appointmentId: number): Observable<Appointment> {
    console.log("OVO JE:");
    console.log(this.apiUrl + 'appointment/cancelAppointment/' + appointmentId);
    return this.http.put<Appointment>(this.apiUrl + 'appointment/cancelAppointment/' + appointmentId, null);
  }

  getFreeAppointments(companyId: number): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(this.apiUrl + 'appointment/getFreeAppointmentsForCompany/' + companyId);
  }

  getCompanyAppointments(companyId: number): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(this.apiUrl + 'appointment/getCompanyAppointments/' + companyId);
  }

  getAppointment(appointmentId: number) : Observable<Appointment> {
    return this.http.get<Appointment>(this.apiUrl + "appointment/getById/" + appointmentId);
  }

  flagAppointmentAsPickedUp(appointmentId: number): Observable<Appointment> {
    return this.http.put<Appointment>(this.apiUrl + 'appointment/flagAsPickedUp/' + appointmentId, null);
  }

  flagAppointmentAsCancelled(appointmentId: number): Observable<Appointment> {
    return this.http.put<Appointment>(this.apiUrl + 'appointment/flagAsCancelled/' + appointmentId, null);
  }

  flagAppointmentAsExpired(appointmentId: number): Observable<Appointment> {
    return this.http.put<Appointment>(this.apiUrl + 'appointment/flagAsExpired/' + appointmentId, null);
  }
}
