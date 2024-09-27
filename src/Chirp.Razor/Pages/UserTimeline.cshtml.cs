using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }
    private int pageSize = 5;

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author, int? page)
    {
        int currentPage = page ?? 1;
        Cheeps = _service.GetCheepsFromAuthor(author,currentPage,pageSize);
        return Page();
    }
}
