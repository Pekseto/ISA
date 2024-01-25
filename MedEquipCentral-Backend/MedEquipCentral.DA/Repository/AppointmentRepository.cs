﻿using MedEquipCentral.BL.Contracts.DTO;
using MedEquipCentral.DA.Contexts;
using MedEquipCentral.DA.Contracts.IRepository;
using MedEquipCentral.DA.Contracts.Model;
using MedEquipCentral.DA.Contracts.Shared;
using Microsoft.EntityFrameworkCore;

namespace MedEquipCentral.DA.Repository
{
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        public DataContext Context
        {
            get { return _dbContext as DataContext; }
        }
        public AppointmentRepository(DbContext dbContext) : base(dbContext) { }

        public async Task<Appointment> GetById(int id)
        {
            return await _dbContext.Set<Appointment>()
                                   .Include(x => x.Admin)
                                   .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Appointment>> GetFreeAppointments(int companyId)
        {
            var result = _dbContext.Set<Appointment>()
                                   .Where(x => x.CompanyId == companyId && x.BuyerId == null)
                                   .Include(x => x.Equipment)
                                   .Include(x => x.Admin)
                                   //.Include(x => x.Buyer)
                                   .ToList();

            return result;
        }

        public async Task<List<Appointment>> GetAllForEquipment(int equipmentId)
        {
            var result = _dbContext.Set<Appointment>()
                                   .Where(x => x.EquipmentIds.Contains(equipmentId))
                                   .Include(x => x.Admin)
                                   .ToList();

            return result;
        }

        public async Task<List<Appointment>> GetAllByCompany(int companyId)
        {
            var result = _dbContext.Set<Appointment>()
                                   .Where(x => x.CompanyId == companyId)
                                   .Include(x => x.Buyer)
                                   .Include(x => x.Equipment)
                                   .Include(x => x.Admin)
                                   .ToList();

            return result;
        }

        public async Task<List<Appointment>> GetAllUsersAppointments(AppointmentPagedIn dataIn)
        {
            var result = _dbContext.Set<Appointment>()
                            .Where(x => x.BuyerId == dataIn.UserId)
                            .Include(x => x.Buyer)
                            .Include(x => x.Equipment)
                            .Include(x => x.Company)
                            .Include(x => x.Admin)
                            .ToList();

            return result;
        }

        public async Task<List<Appointment>> GetAllAdminsAppointments(int adminId)
        {
            var result = _dbContext.Set<Appointment>()
                            .Where(x => x.AdminId == adminId)
                            .Include(x => x.Buyer)
                            .Include (x => x.Equipment)
                            .Include(x => x.Company)
                            .Include(x => x.Admin)
                            .ToList();

            return result;
        }

        public async Task<List<Appointment>> GetUncollectedAppointments()
        {
            var result = _dbContext.Set<Appointment>()
                            .Where(x => x.IsCollected == false && x.BuyerId != null && x.StartTime.AddMinutes(x.Duration) <= DateTime.UtcNow)
                            .Include(x => x.Buyer)
                            .ToList();
            return result;
        }
    }
}
