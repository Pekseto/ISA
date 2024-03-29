﻿using MedEquipCentral.DA.Contracts.Model;
using MedEquipCentral.DA.Contracts.Shared;

namespace MedEquipCentral.DA.Contracts.Helper
{
    public class QrCode : Entity
    {
        public int AdminId { get; set; }
        public int? BuyerId { get; set; }
        public int AppointmentId { get; set; }
        public AppointmentStatus AppointmentStatus { get; set; }
        public string Path { get; set; }
    }
}
