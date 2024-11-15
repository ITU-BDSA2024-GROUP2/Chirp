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
    public List<CheepDTO> Cheeps { get; set; }

    [BindProperty]
    public CheepViewModel CheepInput { get; set; } 


    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        FollowerMap = new Dictionary<string, bool>();
    }

    public async Task<ActionResult> OnGet(string author,[FromQuery] int? page)
    {
        int currentPage = page ?? 1;
        Cheeps = await _cheepRepository.GetCheepsFromFollowers(author, currentPage);
        
        if (User.Identity.IsAuthenticated)
        {
            foreach (var cheep in Cheeps)
            {
                FollowerMap[cheep.Author] = await IsFollowing(User.Identity.Name, cheep.Author);
            }
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
            Cheeps = await _cheepRepository.GetCheeps(1);
            return Page();
        }

        await _cheepRepository.CreateCheep(User.Identity.Name, CheepInput.Message);
        return RedirectToPage("UserTimeline");
    }
    
    public async Task<IActionResult> OnPostFollow(string authorName)
    {
        await _authorRepository.Follow(User.Identity.Name, authorName);
        
        FollowerMap[authorName] = true;
        return RedirectToPage("UserTimeline");
    }
    
    public async Task<IActionResult> OnPostUnfollow(string authorName)
    {
        await _authorRepository.Unfollow(User.Identity.Name, authorName);
        
        FollowerMap[authorName] = false;
        return RedirectToPage("UserTimeline");
    }

    public async Task<bool> IsFollowing(string userName, string authorName)
    {
        return await _authorRepository.IsFollowing(userName, authorName);
    }
}
