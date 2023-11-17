﻿using MedEquipCentral.DA.Contexts;
using MedEquipCentral.DA.Contracts.IRepository;
using MedEquipCentral.DA.Contracts.Model;
using Microsoft.EntityFrameworkCore;

namespace MedEquipCentral.DA.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public DataContext Context
        {
            get { return _dbContext as DataContext; }
        }

        public CompanyRepository(DataContext context) : base(context) { }

        public new IEnumerable<Company> GetAll()
        {
            return _dbContext.Set<Company>().Include(c => c.Location).ToList();
        }

    }
}
