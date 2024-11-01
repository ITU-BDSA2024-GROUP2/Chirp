#nullable disable //fjern null warning
using System.ComponentModel.DataAnnotations;
using Azure;
using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; }

    [BindProperty]
    public CheepViewModel CheepInput { get; set; } 


    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public async Task<ActionResult> OnGet(string author,[FromQuery] int? page)
    {
        int currentPage = page ?? 1;
        
        Cheeps = await _service.GetCheepsFromAuthor(author, currentPage);
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
        return RedirectToPage("UserTimeline");
    }
}
