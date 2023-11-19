﻿using MedEquipCentral.DA.Contracts.Model;

namespace MedEquipCentral.DA.Contracts.IRepository
{
    public interface IEquipmentRepository : IRepository<Equipment>
    {
        Task<List<Equipment>> GetAllForCompany(int companyId);
        List<Equipment> GetAll();
    }
}
