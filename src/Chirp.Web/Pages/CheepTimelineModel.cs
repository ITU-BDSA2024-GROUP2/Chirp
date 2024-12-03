using Azure;
using Chirp.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class CheepTimelineModel : PageModel
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;
    private readonly UserManager<Author> _userManager;
    public IEnumerable<CheepDTO> Cheeps { get; set; }
    public Dictionary<string, string?> AuthorAvatars { get; private set; } = new();
    
    public CheepTimelineModel(UserManager<Author> userManager, ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _userManager = userManager;
        Cheeps = new List<CheepDTO>();
    }

    public async Task<IActionResult> OnGet([FromQuery] int? page)
    {
        var currentpage = page ?? 1;
        Cheeps = await _cheepRepository.GetCheepsByNewest(currentpage);
        await GetAvatar();
        
        Console.WriteLine("I have updated Cheeps");
        return Page();
    }

    public async Task GetAvatar()
    {
        foreach (var cheep in Cheeps)
        {
            var avatar = await _authorRepository.GetProfilePicture(cheep.Author);
            AuthorAvatars[cheep.Author] = avatar;
        }
    }
    
}