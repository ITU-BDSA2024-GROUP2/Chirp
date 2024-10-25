using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; }

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
}
