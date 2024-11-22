using Chirp.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Areas.Identity.Pages.Account.AboutMe
{
    public class InfoPage : PageModel
    {
        private readonly SignInManager<Author> _signInManager;

        public string ProviderDisplayName { get; set; } = "N/A";
        public string LoginProvider { get; set; } = "Default";

        public InfoPage(SignInManager<Author> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<ActionResult> OnGet()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            Console.WriteLine("PRINT STATEMENT START------------");
            if (info != null)
            {
                Console.WriteLine($"LoginProvider: {info.LoginProvider}");
                Console.WriteLine($"ProviderDisplayName: {info.ProviderDisplayName}");

                ProviderDisplayName = info.ProviderDisplayName ?? "N/A";
                LoginProvider = info.LoginProvider ?? "Default";
            }
            else
            {
                Console.WriteLine("No external login information available.");
            }
            Console.WriteLine("PRINT STATEMENT END------------");

            return Page();
        }
    }
}