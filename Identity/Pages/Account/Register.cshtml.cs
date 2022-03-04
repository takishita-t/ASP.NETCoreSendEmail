using System.Net;
using System.Net.Mail;
using Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Identity.Data.Account;
using System.Security.Claims;
using MimeKit;
using MailKit.Security;

namespace Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly IEmailService emailService;

        public RegisterModel(UserManager<User> userManager,
            IEmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Validating Email address (Optional)

            // Create the user 
            var user = new User
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email,
                TwoFactorEnabled = true
            };

            var claimDepartment = new Claim("Department", RegisterViewModel.Department);
            var claimPosition = new Claim("Position", RegisterViewModel.Position);

            var result = await this.userManager.CreateAsync(user, RegisterViewModel.Password);
            if (result.Succeeded)
            {
                await this.userManager.AddClaimAsync(user, claimDepartment);
                await this.userManager.AddClaimAsync(user, claimPosition);

                var confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail",
                    values: new { userId = user.Id, token = confirmationToken });

                //send mail
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("", "test@example.com"));

                emailMessage.To.Add(new MailboxAddress("test@test.com", "test@test.com"));

                emailMessage.Subject = "Confirm your email";

                emailMessage.Body = new TextPart("plain") { Text = $"Confirm your email {confirmationLink}" };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync("localhost", 1025, SecureSocketOptions.Auto);
                    //await client.AuthenticateAsync(_sendMailParams.User, _sendMailParams.Password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

                return RedirectToPage("/Account/Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

                return Page();
            }
        }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage ="Invalid email address.")]
        public string Email { get; set; }

        [Required]
        [DataType(dataType:DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string Position { get; set; }
    }
}
