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
    public int _currentPage;
    public bool _nextPageHasCheeps;
    public List<CheepDTO> Cheeps { get; set; }

    [BindProperty]
    public CheepInputModel CheepInput { get; set; } 


    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        FollowerMap = new Dictionary<string, bool>();
    }

    public async Task<ActionResult> OnGet(string author, [FromQuery] int? page)
    {
        _currentPage = page ?? 1;
        ViewData["CurrentPage"] = _currentPage;
        
        await PopulateCheepsAndFollowers(author, _currentPage);
        _nextPageHasCheeps = await NextPageHasCheeps(author, _currentPage);
        
        return Page();
        
    }
    
    public async Task<IActionResult> OnPost(string author)
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
            await PopulateCheepsAndFollowers(author, _currentPage);
            return Page();
        }

        await _cheepRepository.CreateCheep(User.Identity.Name, CheepInput.Message);
        return RedirectToPage("UserTimeline", new { page = _currentPage });
    }
    
    public async Task<IActionResult> OnPostDelete(string cheepId)
    {
        await _cheepRepository.DeleteCheep(cheepId);
        return RedirectToPage("UserTimeline");
    }
    
    public async Task<IActionResult> OnPostFollow(string author, string authorName, int? page)
    {
        if (User.Identity.Name == authorName)
        {
            ModelState.AddModelError(string.Empty, "You cannot follow yourself.");
            return Redirect($"/?page={page}");
        }
        
        await _authorRepository.Follow(User.Identity.Name, authorName);
        
        FollowerMap[authorName] = true;
        return LocalRedirect($"/{author}?page={page}");
    }
    
    public async Task<IActionResult> OnPostUnfollow(string author, string authorName, int? page)
    {
        await _authorRepository.Unfollow(User.Identity.Name, authorName);
        
        FollowerMap[authorName] = false;
        return LocalRedirect($"/{author}?page={page}");
    }

    public async Task<bool> IsFollowing(string userName, string authorName)
    {
        return await _authorRepository.IsFollowing(userName, authorName);
    }
    
    private async Task PopulateCheepsAndFollowers(string author, int page)
    {
        if (author == User.Identity.Name)
        {
            Cheeps = await _cheepRepository.GetCheepsFromFollowersAndOwnCheeps(author, page);
        }
        else
        {
            Cheeps = await _cheepRepository.GetCheepsFromAuthor(author, page);
        }

        if (User.Identity.IsAuthenticated)
        {
            foreach (var cheep in Cheeps)
            {
                if (cheep.Author != User.Identity.Name)
                {
                    FollowerMap[cheep.Author] = await IsFollowing(User.Identity.Name, cheep.Author);
                }
            }
        }
    }
    
    public async Task<bool> NextPageHasCheeps(string author, int page)
    {   
        if (author == User.Identity.Name)
        {
            var list = await _cheepRepository.GetCheepsFromFollowersAndOwnCheeps(author, page+1);
            return list.Any();
        }
        else
        {
            var list = await _cheepRepository.GetCheepsFromAuthor(author, page+1);
            return list.Any();
        }
    }
}
