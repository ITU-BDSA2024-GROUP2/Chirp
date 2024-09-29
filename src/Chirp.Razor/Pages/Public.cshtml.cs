using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    private const int PageSize = 32;

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet([FromQuery] int? page)
    {
        var currentPage = page ?? 1;
        
        Cheeps = _service.GetCheeps(currentPage, PageSize);
        return Page();
    }
}
