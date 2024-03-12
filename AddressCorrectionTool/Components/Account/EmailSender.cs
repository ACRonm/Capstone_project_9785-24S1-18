using AddressCorrectionTool.Data;
using Microsoft.AspNetCore.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Options;
using AddressCorrectionTool.Services;

namespace AddressCorrectionTool.Components.Account
{
    public class EmailSender : IEmailSender<ApplicationUser>
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                           ILogger<EmailSender> logger)
        {
            _logger = logger;
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; } //Set with Secret Manager.

        public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            Console.WriteLine("Sending confirmation email");
            await SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
        }

        public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            Console.WriteLine("Sending password reset email");
            await SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
        }

        public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            Console.WriteLine("Sending password reset email");
            await SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
        }

        private async Task SendEmailAsync(string email, string subject, string htmlContent)
        {

            Console.WriteLine("Sending email");
            var client = new SendGridClient(Options.SendGridKey);
            var from = new EmailAddress("no-reply@aidenr.dev", "Address Correction Tool");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            await client.SendEmailAsync(msg);
        }
    }
}
