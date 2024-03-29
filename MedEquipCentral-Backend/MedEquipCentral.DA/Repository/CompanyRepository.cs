﻿using MedEquipCentral.BL.Contracts.DTO;
using MedEquipCentral.DA.Contexts;
using MedEquipCentral.DA.Contracts.IRepository;
using MedEquipCentral.DA.Contracts.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

        public async Task<List<Company>> GetAllBySearch(CompanyPagedIn dataIn)
        {
            var search = dataIn.CompanyFilter.Search;
            var query = _dbContext.Set<Company>().Include(x => x.Location).AsQueryable();
            if (!search.IsNullOrEmpty())
            {
                query = _dbContext.Set<Company>().Include(x => x.Location).Where(x => x.Name == search || x.Location.City == search);
            }

            if(dataIn.CompanyFilter.Rating != 0)
            {
                query = query.Where(x => x.Rating <= dataIn.CompanyFilter.Rating);
            }

            if (dataIn.CompanyFilter.SortBy == "Name" && dataIn.CompanyFilter.IsAsc)
            {
                query = query.OrderBy(x => x.Name);
            }
            else if (dataIn.CompanyFilter.SortBy == "Name" && !dataIn.CompanyFilter.IsAsc)
            {
                query = query.OrderByDescending(x => x.Name);
            }
            else if (dataIn.CompanyFilter.SortBy == "City" && dataIn.CompanyFilter.IsAsc)
            {
                query = query.OrderBy(x => x.Location.City);
            }
            else if (dataIn.CompanyFilter.SortBy == "City" && !dataIn.CompanyFilter.IsAsc)
            {
                query = query.OrderByDescending(x => x.Location.City);
            }
            else if (dataIn.CompanyFilter.SortBy == "Rating" && dataIn.CompanyFilter.IsAsc)
            {
                query = query.OrderBy(x => x.Rating);
            }
            else if (dataIn.CompanyFilter.SortBy == "Rating" && !dataIn.CompanyFilter.IsAsc)
            {
                query = query.OrderByDescending(x => x.Rating);
            }

            return query.ToList();
        }
    }
}
