using Chirp.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Areas.Identity.Pages.Account.AboutMe;

public class InfoPage : PageModel
{
    private readonly SignInManager<Author> _signInManager;
    public string ProviderDisplayName { get; set; }
    public string LoginProvider { get; set; }
    
    public InfoPage(SignInManager<Author> signInManager)
    {
          _signInManager = signInManager;
          
    }
    
    public async Task<ActionResult> OnGet()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        Console.WriteLine("PRINT STATEMENT START------------");
        Console.WriteLine(info.LoginProvider);
        Console.WriteLine("PRINT STATEMENT END------------");
        ProviderDisplayName = info.ProviderDisplayName;
        LoginProvider = info.LoginProvider;
        
        return Page();
    }
}