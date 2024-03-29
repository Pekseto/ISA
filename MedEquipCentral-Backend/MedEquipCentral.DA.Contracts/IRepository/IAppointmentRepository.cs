﻿using MedEquipCentral.BL.Contracts.DTO;
using MedEquipCentral.DA.Contracts.Model;

namespace MedEquipCentral.DA.Contracts.IRepository
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<List<Appointment>> GetFreeAppointments(int companyId);
        Task<List<Appointment>> GetAllForEquipment(int equipmentId);
        Task<List<Appointment>> GetAllByCompany(int companyId);
        Task<List<Appointment>> GetAllUsersAppointments(AppointmentPagedIn dataIn);
        Task<Appointment> GetById(int id);
        Task<List<Appointment>> GetAllAdminsAppointments(int adminId);
        Task<List<Appointment>> GetUncollectedAppointments(int adminId);
        Task<Appointment> Remove(Appointment appointment);
        Task<List<Appointment>> GetHistory(AppointmentPagedIn dataIn);
    }
}
