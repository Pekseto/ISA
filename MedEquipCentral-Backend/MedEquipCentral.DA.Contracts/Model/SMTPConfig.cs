﻿namespace MedEquipCentral.DA.Contracts.Model;
public class SMTPConfig
{
    public string SenderAddress { get; set; }
    public string SenderDisplayName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public bool EnableSSL { get; set; }
    public bool UseDefaultCredentials { get; set; }
    public bool IsBodyHTML { get; set; }
}