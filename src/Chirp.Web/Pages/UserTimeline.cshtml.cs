#nullable disable //fjern null warning
using System.ComponentModel.DataAnnotations;
using Azure;
using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;

    public Dictionary<string, bool> FollowerMap;
    private int _currentPage;
    public List<CheepDTO> Cheeps { get; set; }

    [BindProperty]
    public CheepViewModel CheepInput { get; set; } 


    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        FollowerMap = new Dictionary<string, bool>();
    }

    public async Task<ActionResult> OnGet(string author, [FromQuery] int? page)
    {
        _currentPage = page ?? 1;
        
        await PopulateCheepsAndFollowers(_currentPage);
        
        return Page();
        
    }
    
    public async Task<IActionResult> OnPost()
    {
        if (string.IsNullOrWhiteSpace(CheepInput.Message))
        {
            ModelState.AddModelError("Message", "Message cannot be empty.");
        }
        else if (CheepInput.Message.Length > 160)
        {
            ModelState.AddModelError("Message", "Message cannot be more 160 characters.");
        }
        if (!ModelState.IsValid)
        {
            await PopulateCheepsAndFollowers(_currentPage);
            return Page();
        }

        await _cheepRepository.CreateCheep(User.Identity.Name, CheepInput.Message);
        return RedirectToPage("UserTimeline", new { page = _currentPage });
    }
    
    public async Task<IActionResult> OnPostFollow(string author, string authorName, int? page)
    {
        _currentPage = page ?? 1;
        
        await _authorRepository.Follow(User.Identity.Name, authorName);
        
        FollowerMap[authorName] = true;
        return LocalRedirect($"/{author}?page={_currentPage}");
    }
    
    public async Task<IActionResult> OnPostUnfollow(string author, string authorName, int? page)
    {
        _currentPage = page ?? 1;
        
        await _authorRepository.Unfollow(User.Identity.Name, authorName);
        
        FollowerMap[authorName] = false;
        return LocalRedirect($"/{author}?page={_currentPage}");
    }

    public async Task<bool> IsFollowing(string userName, string authorName)
    {
        return await _authorRepository.IsFollowing(userName, authorName);
    }
    
    private async Task PopulateCheepsAndFollowers(int page)
    {
        Cheeps = await _cheepRepository.GetCheeps(page);

        if (User.Identity.IsAuthenticated)
        {
            foreach (var cheep in Cheeps)
            {
                FollowerMap[cheep.Author] = await IsFollowing(User.Identity.Name, cheep.Author);
            }
        }
    }
}
