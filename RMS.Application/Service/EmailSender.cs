using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using RMS.Domain.Models;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace RMS.Application.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _cofiguration;
        public EmailSender(IConfiguration configuration)
        {
            _cofiguration = configuration;
        } 
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                    var senderEmail = _cofiguration["UserCredintials:Email"];
                    var senderPassword = _cofiguration["UserCredintials:Password"];
                //Logic to send email
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail,senderPassword)
                };
                var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, "Kimchi Resturant"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                message.To.Add(email);
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                throw new ApplicationException("Failed to send email.", ex);
            }
        }


    }
}
