using Chirp.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Areas.Identity.Pages.Account.AboutMe;

public class InfoPage : PageModel
{
    private readonly SignInManager<Author> _signInManager;
    
    public string ProviderDisplayName { get; set; }
    
    public async void OnGet()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        ProviderDisplayName = info.ProviderDisplayName;
    }
}