using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; }
    private int pageSize = 32;

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public async Task<ActionResult> OnGet(string author,[FromQuery] int? page)
    {
        int currentPage = page ?? 1;
        
        Cheeps = await _service.GetCheepsFromAuthor(author,currentPage, pageSize);
        return Page();
    }
}
