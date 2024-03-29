﻿#nullable disable

namespace MedEquipCentral.BL.Contracts.DTO
{
    public class UserDto
    {
        public int Id { get; set; } = 0;
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Job { get; set; }
        public string CompanyInfo { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsFirstLogin { get; set; }
        public int? PenalPoints { get; set; }
        //TODO Findout how to import UserRole
        //public UserRole Role { get; set; }
    }
}
