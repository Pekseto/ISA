﻿using AutoMapper;
using IronBarCode;
using MedEquipCentral.BL.Contracts.DTO;
using MedEquipCentral.BL.Contracts.IService;
using MedEquipCentral.DA.Contracts;
using MedEquipCentral.DA.Contracts.Helper;
using MedEquipCentral.DA.Contracts.Model;
using MedEquipCentral.DA.Contracts.Shared;
using AppointmentStatus = MedEquipCentral.BL.Contracts.DTO.AppointmentStatus;

namespace MedEquipCentral.BL.Service
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IMailKitService _mailKitService;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IMailKitService mailKitService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _mailKitService = mailKitService;
        }

        public async Task<string> CreateAppointment(AppointmentDto dataIn)
        {
            var result = await AddAppointment(dataIn);

            if (result != null && result.BuyerId != null)
            {
                await CreateQRCodeForAppointment(result);
                return "Appointment created successfully, QR code is sent to your email";
            }
            else if (result != null)
            {
                return "Appointment created successfully";
            }

            return "Appointment creation failed";

        }

        private async Task<AppointmentDto> AddAppointment(AppointmentDto appointmentDto)
        {
            //Working hours validation
            var company = await _unitOfWork.GetCompanyRepository().GetByIdAsync(appointmentDto.CompanyId);
            var appointmentTime = TimeOnly.FromDateTime(appointmentDto.StartTime);
            if (!(appointmentTime >= company.StartTime && appointmentTime <= company.EndTime && appointmentTime.AddMinutes(appointmentDto.Duration) <= company.EndTime)) return null;

            //User creation
            if (appointmentDto.AdminId == 0)
            {
                var companyAdmins = _unitOfWork.GetUserRepository().GetAllByCompanyId(appointmentDto.CompanyId);
                foreach (var companyAdmin in companyAdmins)
                {
                    var appointments = await _unitOfWork.GetAppointmentRepository().GetAllAdminsAppointments(companyAdmin.Id);
                    if (appointments.Count != 0)
                    {
                        foreach (var appointment in appointments)
                        {
                            if (appointmentDto.StartTime.AddMinutes(appointmentDto.Duration) <= appointment.StartTime || appointment.StartTime.AddMinutes(appointment.Duration) < appointmentDto.StartTime)
                            {
                                appointmentDto.AdminId = companyAdmin.Id;
                                var appo = _mapper.Map<Appointment>(appointmentDto);
                                await _unitOfWork.GetAppointmentRepository().Add(appo);

                                if (await Validate(appointmentDto))
                                {
                                    await _unitOfWork.Save();
                                    return appointmentDto;
                                }
                            }
                        }
                    }
                    else
                    {
                        appointmentDto.AdminId = companyAdmin.Id;
                        var app = _mapper.Map<Appointment>(appointmentDto);
                        await _unitOfWork.GetAppointmentRepository().Add(app);

                        if (await Validate(appointmentDto))
                        {
                            await _unitOfWork.Save();
                            return appointmentDto;
                        }
                    }

                }
            }
            else
            {   //Admin creation                  
                var appointment = _mapper.Map<Appointment>(appointmentDto);
                await _unitOfWork.GetAppointmentRepository().Add(appointment);
                if (await Validate(appointmentDto))
                {
                    await _unitOfWork.Save();
                    return appointmentDto;
                }
            }
            return null;
        }

        private async Task<bool> Validate(AppointmentDto appointmentDto)
        {
            var companyAppointments = await _unitOfWork.GetAppointmentRepository().GetAllByCompany(appointmentDto.CompanyId);
            var isValidTime = !companyAppointments.Any(appointment =>
                !(appointmentDto.StartTime.AddMinutes(appointmentDto.Duration) <= appointment.StartTime
                || appointment.StartTime.AddMinutes(appointment.Duration) <= appointmentDto.StartTime));
            return isValidTime;
        }

        public async Task<List<AppointmentDto>> GetFreeAppointmentsForCompany(int companyId)
        {
            var appointments = await _unitOfWork.GetAppointmentRepository().GetFreeAppointments(companyId);
            return _mapper.Map<List<AppointmentDto>>(appointments);
        }

        private async Task<string> CreateQRCodeForAppointment(AppointmentDto appointmentDto)
        {
            var list = await _unitOfWork.GetAppointmentRepository().GetAll();
            var count = list.Count();
            var buyerId = appointmentDto.BuyerId.HasValue == false ? 5 : appointmentDto.BuyerId.Value;
            var user = await _unitOfWork.GetUserRepository().GetByIdAsync(buyerId);
            CreateQrCode(appointmentDto, count);
            await _emailService.SendAppointmentConfirmationEmail(new UserEmailOptionsDto
            {
                ToEmail = user.Email,
                Placeholders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{UserName}}", user.Name),
                    new KeyValuePair<string, string>("{{Id}}", appointmentDto.Id.ToString()),
                }
            }, $"../../MedEquipCentral-Frontend/MedEquipCentral/src/assets/QRCodes/reservation{count}.png");

            var path = $"reservation{count}.png";

            var qrCode = new QrCode();
            qrCode.AdminId = appointmentDto.AdminId;
            qrCode.BuyerId = buyerId;
            qrCode.AppointmentId = count;
            qrCode.AppointmentStatus = (DA.Contracts.Model.AppointmentStatus)AppointmentStatus.NEW;
            qrCode.Path = path;
            await _unitOfWork.GetQrCodeRepository().Add(qrCode);
            await _unitOfWork.Save();
            return null;
        }

        private static void CreateQrCode(AppointmentDto appointmentDto, int count)
        {
            //TODO ispis liste doraditi
            //TODO uraditi relativne putanje
            var appointment = $"http://localhost:4200/appointment-details/{count}";
            QRCodeLogo qrCodeLogo = new QRCodeLogo("../../favicon.png");
            GeneratedBarcode myQRCodeWithLogo = QRCodeWriter.CreateQrCodeWithLogo(appointment, qrCodeLogo);
            myQRCodeWithLogo.ResizeTo(500, 500).SetMargins(10).ChangeBarCodeColor(Color.DarkBlue);
            myQRCodeWithLogo.SaveAsPng($"../../MedEquipCentral-Frontend/MedEquipCentral/src/assets/QRCodes/reservation{count}.png");
        }

        public async Task<List<AppointmentDto>> GetCompanyAppointments(int companyId)
        {
            var appointments = await _unitOfWork.GetAppointmentRepository().GetAllByCompany(companyId);
            return _mapper.Map<List<AppointmentDto>>(appointments);
        }

        public async Task<AppointmentDto> GetById(int appointmentId)
        {
            var appointment = await _unitOfWork.GetAppointmentRepository().GetById(appointmentId);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);

            return appointmentDto;
        }

        public async Task<PagedResult<AppointmentDto>> GetAllUsersAppointments(AppointmentPagedIn dataIn)
        {
            var appointments = !dataIn.IsAdmin
                ? await _unitOfWork.GetAppointmentRepository().GetAllUsersAppointments(dataIn)
                : await _unitOfWork.GetAppointmentRepository().GetAllAdminsAppointments(dataIn.UserId);

            var appointmentsDto = _mapper.Map<List<AppointmentDto>>(appointments);

            return new PagedResult<AppointmentDto>()
            {
                Result = appointmentsDto
            };
        }

        public async Task<bool> FlagAs(int appointmentId, AppointmentStatus status)
        {
            var appointment = await _unitOfWork.GetAppointmentRepository().GetById(appointmentId);
            if (appointment is null)
            {
                return false;
            }
            appointment.Status = (DA.Contracts.Model.AppointmentStatus?)status;

            var qrCode = await _unitOfWork.GetQrCodeRepository().GetByAppointmentId(appointmentId);
            qrCode.AppointmentStatus = (DA.Contracts.Model.AppointmentStatus)status;
            _unitOfWork.GetQrCodeRepository().Update(qrCode);

            await _unitOfWork.Save();

            if (status == AppointmentStatus.PROCESSED)
            {
                if (appointment.BuyerId is null)
                {
                    return false;
                }
                var user = await _unitOfWork.GetUserRepository().GetByIdAsync((int)appointment.BuyerId);
                _mailKitService.SendPickupConfirmEmail(user.Email);
            }

            if (status == AppointmentStatus.EXPIRED)
            {
                if (appointment.BuyerId is null)
                {
                    return false;
                }
                var user = await _unitOfWork.GetUserRepository().GetByIdAsync((int)appointment.BuyerId);
                user.PenalPoints += 2;
                _unitOfWork.GetUserRepository().Update(user);
                await _unitOfWork.Save();
            }

            return true;
        }

        public async Task<string> CancelAppointment(int appointmentId)
        {
            var appointmentDb = await _unitOfWork.GetAppointmentRepository().GetByIdAsync(appointmentId);
            var appointment = await _unitOfWork.GetAppointmentRepository().GetByIdAsync(appointmentId);
            appointment.Status = DA.Contracts.Model.AppointmentStatus.CANCELLED;

            var timeDifference = DateTime.Now.Subtract(appointmentDb.StartTime).TotalHours;
            var user = await _unitOfWork.GetUserRepository()?.GetByIdAsync((int)appointment.BuyerId);

            var qrCode = await _unitOfWork.GetQrCodeRepository().GetByAppointmentId(appointmentId);
            qrCode.AppointmentStatus = (DA.Contracts.Model.AppointmentStatus)AppointmentStatus.CANCELLED;

            if (timeDifference > 24)
            {
                user.PenalPoints += 1;
                _unitOfWork.GetAppointmentRepository().Update(appointment);
                _unitOfWork.GetUserRepository().Update(user);
                await _unitOfWork.Save();
                return "Appointment canceled successfully";
            }
            else
            {
                user.PenalPoints += 2;
                _unitOfWork.GetAppointmentRepository().Update(appointment);
                _unitOfWork.GetUserRepository().Update(user);
                await _unitOfWork.Save();
                return "Appointment canceled successfully";
            }
        }

        public async Task<AppointmentDto> Update(AppointmentDto appointmentDto)
        {
            //Validacija da li je vec rezervisan
            var appointment = await _unitOfWork.GetAppointmentRepository().GetByIdAsync(appointmentDto.Id);
            if (appointment.BuyerId != null) return null;

            appointment.Status = (DA.Contracts.Model.AppointmentStatus?)appointmentDto.Status;
            appointment.EquipmentIds= appointmentDto.EquipmentIds;

            bool isAllow = true;
            if(appointment.EquipmentIds.Count > 0 || appointment.EquipmentIds != null)
            {
                foreach(var eqId in appointmentDto.EquipmentIds)
                {
                    var equip = await _unitOfWork.GetEquipmentRepository().GetByIdAsync(eqId);
                    equip.Reserved += 1;
                    if(equip.Quantity >= equip.Reserved)
                    {
                        isAllow = false;
                    }
                }
            }
            appointment.BuyerId= appointmentDto.BuyerId;
            _unitOfWork.GetAppointmentRepository().Update(appointment);

            var qrCode = await _unitOfWork.GetQrCodeRepository().GetByAppointmentId(appointmentDto.Id);
            qrCode.BuyerId = appointmentDto.BuyerId;
            _unitOfWork.GetQrCodeRepository().Update(qrCode);

            if (isAllow)
            {
                await _unitOfWork.Save();
            }
            return appointmentDto;
        }

        public async Task SendCollectionConfirmationEmail(AppointmentDto appointmentDto)
        {
            await _emailService.SendCollectionConfirmationEmail(new UserEmailOptionsDto
            {
                ToEmail = appointmentDto.Buyer.Email,
                Placeholders = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("{{CompanyName}}", appointmentDto.Company.Name),
            }
            });
        }

        public async Task<List<AppointmentDto>> GetHistory(AppointmentPagedIn dataIn)
        {
            var appointments = await _unitOfWork.GetAppointmentRepository().GetHistory(dataIn);

            var appointmentDtos = _mapper.Map<List<AppointmentDto>>(appointments);

            return appointmentDtos;
        }

        public async Task<List<QrCodeDto>> GetQrCodes(QrCodeDataIn dataIn)
        {
            var qrCodes = await _unitOfWork.GetQrCodeRepository().GetByUserId(dataIn);

            var qrCodeDtos = _mapper.Map<List<QrCodeDto>>(qrCodes);

            return qrCodeDtos;
        }
    }
}
