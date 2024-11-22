using Chirp.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Areas.Identity.Pages.Account.AboutMe
{
    public class InfoPage : PageModel
    {
        private readonly UserManager<Author> _userManager;

        public string ProviderDisplayName { get; set; } = "N/A";
        public string LoginProvider { get; set; } = "Default";

        public InfoPage(UserManager<Author> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Retrieve external logins for the user
            var logins = await _userManager.GetLoginsAsync(user);
            if (logins.Any())
            {
                var externalLogin = logins.First();
                LoginProvider = externalLogin.LoginProvider;
                ProviderDisplayName = externalLogin.ProviderDisplayName ?? "External Provider";
            }

            return Page();
        }
    }
}