﻿#nullable disable //fjern null warning
using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; }
    
    [BindProperty]
    public SubmitMessageModel SubmitMessage { get; set; }

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
}
