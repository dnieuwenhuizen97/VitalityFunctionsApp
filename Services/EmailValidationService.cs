﻿using SendGrid;
using SendGrid.Helpers.Mail;
using Services.Interfaces;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Services
{
    public class EmailValidationService : IEmailValidationService
    {
        public async Task<bool> ValidateEmployeeEmail(string Email)
        {
            string pattern = @"([a-zA-Z0-9_\-]+)\.([a-zA-Z0-9_\-]+)@inholland.nl";

            Regex regex = new Regex(pattern);

            return regex.IsMatch(Email);
        }

        public async Task SendValidationEmail(string Email, string userId)
        {
            string currentHost = Environment.GetEnvironmentVariable("HostAddress");

            Uri uri = new Uri($"{currentHost}api/user/verify?userId={userId}");
            Console.WriteLine(uri);
            try
            {
                SendGridClient client = new SendGridClient(Environment.GetEnvironmentVariable("SendGridClient"));
                EmailAddress from = new EmailAddress(Environment.GetEnvironmentVariable("SendGridEmailAddress"), "Inholland MijnVitaliteit");
                string subject = "Activeer je MijnVitaliteit account";
                EmailAddress to = new EmailAddress(Email, "");
                string plainTextContent = "Bedankt voor het registeren in de Inholland MijnVitaliteit app. Om gebruik te kunnen maken van de app moet jouw account geactiveerd worden via deze link.";
                string htmlContent = $"<div><strong>Bedankt voor het registeren in de Inholland MijnVitaliteit app</strong><br>" +
                                    $"<p>Om gebruik te kunnen maken van de app moet jouw account geactiveerd worden via <a href={uri}>Deze link</a>.</p></div>";
                SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                Response response = await client.SendEmailAsync(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task SendRecoveryEmail(string email, string recoveryToken)
        {
            string passRecoveryHost = Environment.GetEnvironmentVariable("PassRecoveryHost");

            Uri uri = new Uri($"{passRecoveryHost}?token={recoveryToken}#/recover");
            Console.WriteLine(uri);
            try
            {
                SendGridClient client = new SendGridClient(Environment.GetEnvironmentVariable("SendGridClient"));
                EmailAddress from = new EmailAddress(Environment.GetEnvironmentVariable("SendGridEmailAddress"), "Inholland MijnVitaliteit");
                string subject = "Herstel je MijnVitaliteit account";
                EmailAddress to = new EmailAddress(email, "");
                string plainTextContent = "";
                string htmlContent = $"<div><p>Om uw wachtwoord te herstellen kunt u gebruik maken van <a href={uri}>deze link</a>.</p></div>";
                SendGridMessage msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                Response response = await client.SendEmailAsync(msg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public Task<bool> ValidatePassword(string Password)
        {
            Regex specialChars = new Regex("[^A-Za-z0-9]");
            Match match = specialChars.Match(Password);

            if (Password != null)
            {
                if (Password.Length >= 8 && !Password.Any(c => char.IsWhiteSpace(c)) && Password.Any(c => char.IsDigit(c)) && Password.Any(c => char.IsUpper(c)) && match.Success)
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }

    }
}
