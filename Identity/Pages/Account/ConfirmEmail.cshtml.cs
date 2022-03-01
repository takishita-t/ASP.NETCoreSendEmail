using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;

        [BindProperty]
        public string Message { get; set; }
        public ConfirmEmailModel(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            var user = await this.userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await this.userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    this.Message = "Email address is successfully confirm, now you can try to login.";
                    return Page();
                }
            }
            this.Message = "Failed to Validate email";
            return Page();
        }
    }
}
