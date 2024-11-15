#nullable disable //fjern null warning
using System.ComponentModel.DataAnnotations;
using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    public List<CheepDTO> Cheeps { get; set; }
    
    public List<Author> Following { get; set; }
    
    [BindProperty]
    public CheepViewModel CheepInput { get; set; }
    
    public PublicModel(ICheepService cheepService, IAuthorService authorService)
    {
        _cheepService = cheepService;
        _authorService = authorService;

    }

    public async Task<ActionResult> OnGet([FromQuery] int? page)
    {
        var currentPage = page ?? 1;
        
        Cheeps = await _cheepService.GetCheeps(currentPage);

        if (User.Identity.IsAuthenticated)
        {
            Following = await _authorService.GetFollowingAuthors(User.Identity.Name);
        }
            
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
            Cheeps = await _cheepService.GetCheeps(1);
            return Page();
        }

        await _cheepService.CreateCheep(User.Identity.Name, CheepInput.Message);
        return RedirectToPage("Public");
    }
    
}
