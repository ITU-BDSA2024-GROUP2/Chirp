#nullable disable //fjern null warning
using System.ComponentModel.DataAnnotations;
using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; }
    
    [BindProperty]
    public CheepViewModel CheepInput { get; set; } 
    
    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public async Task<ActionResult> OnGet([FromQuery] int? page)
    {
        var currentPage = page ?? 1;
        
        Cheeps = await _service.GetCheeps(currentPage);
        return Page();
    }
    
    public async Task<IActionResult> OnPost()
    {
        if (string.IsNullOrWhiteSpace(CheepInput.Message))
        {
            ModelState.AddModelError("CheepInput.Message", "Message cannot be empty.");
        }
        else if (CheepInput.Message.Length > 160)
        {
            ModelState.AddModelError("CheepInput.Message", "Message cannot be more 160 characters.");
        }
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _service.CreateCheep(User.Identity.Name, CheepInput.Message);
        return RedirectToPage("Public");
    }
}
