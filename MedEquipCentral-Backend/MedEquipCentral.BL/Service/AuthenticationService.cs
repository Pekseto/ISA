﻿using MedEquipCentral.BL.Contracts.DTO;
using MedEquipCentral.BL.Contracts.IService;
using MedEquipCentral.DA.Contracts;
using MedEquipCentral.DA.Contracts.Model;
using MedEquipCentral.DA.Contracts.Shared;
using Microsoft.Extensions.Configuration;
using System;

namespace MedEquipCentral.BL.Service;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthenticationService(IUnitOfWork unitOfWork, IEmailService emailService, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<AuthenticationTokensDto> ConfirmEmailAsync(int userId)
    {
        var user = await _unitOfWork.GetUserRepository().GetByIdAsync(userId);
        user.Role = UserRole.Registered;
        user = _unitOfWork.GetUserRepository().Update(user);
        await _unitOfWork.Save();
        return await _unitOfWork.GetTokenGeneratorRepository().GenerateAccessToken(user);
    }

    public async Task<AuthenticationTokensDto?> Login(CredentialsDto credentials)
    {
        var user = await _unitOfWork.GetUserRepository().GetByEmailAsync(credentials.Username);
        if(user != null && _unitOfWork.GetUserRepository().CheckPasswordAsync(user, credentials.Password)) 
        {
            return await _unitOfWork.GetTokenGeneratorRepository().GenerateAccessToken(user);
        }
        return null;
    }

    public async Task<AuthenticationTokensDto>? RegisterUser(UserDto userDto)
    {
        string appDomain = _configuration.GetSection("Application:AppDomain").Value;
        string confirmationLink = _configuration.GetSection("Application:EmailConfirmation").Value;
        var userdb = await _unitOfWork.GetUserRepository().GetByEmailAsync(userDto.Email);
        if(userdb != null)
        {
            return null;
        }

        await _unitOfWork.GetUserRepository().Add(new User(userDto.Email, userDto.Password, userDto.Name, userDto.Surname, userDto.City, userDto.Country, userDto.Phone, userDto.Job, userDto.CompanyInfo, UserRole.Unauthenticated));
        await _unitOfWork.Save();
        var user = await _unitOfWork.GetUserRepository().GetByEmailAsync(userDto.Email);
        var token = await _unitOfWork.GetTokenGeneratorRepository().GenerateAccessToken(user);
        await _emailService.SendVerficationEmail(new UserEmailOptionsDto
        {
            ToEmail = userDto.Email,
            Placeholders = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("{{UserName}}", user.Name),
                new KeyValuePair<string, string>("{{Link}}", string.Format(appDomain) + string.Format(confirmationLink) + string.Format(user.Id.ToString()))
            }
        });
        return token;
    }
}
