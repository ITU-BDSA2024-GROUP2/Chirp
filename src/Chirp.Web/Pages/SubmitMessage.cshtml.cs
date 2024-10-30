using System.ComponentModel.DataAnnotations;
using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class SubmitMessageModel : PageModel
{
    [BindProperty]
    [Required]
    [StringLength(160, ErrorMessage = "Maximum length is {1} characters")]
    [Display(Name = "Message Text")]
    public string Message { get; set; }
    
    private readonly ICheepService _service;

    public SubmitMessageModel(ICheepService service)
    {
        _service = service;
    }

    
    public void OnGet()
    {
        
    }
    
    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _service.CreateCheep(User.Identity.Name, Message);
        return RedirectToPage("Public");
    }
}