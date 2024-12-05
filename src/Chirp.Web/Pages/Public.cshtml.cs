#nullable disable //fjern null warning
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Azure.Identity;
using Chirp.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly UserManager<Author> _userManager;
    public int _currentPage;
    public bool _nextPageHasCheeps;
    public CheepTimelineModel CheepTimelineModel { get; private set; } = new();
    [BindProperty]
    public CheepInputModel CheepInput { get; set; }
    
    public PublicModel(UserManager<Author> userManager, ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _userManager = userManager;
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }

    public async Task<ActionResult> OnGet([FromQuery] int? page)
    {
        _currentPage = page ?? 1;
        CheepTimelineModel.Cheeps = await GetCheeps(_currentPage);
        await FetchAuthorData();
        _nextPageHasCheeps = await NextPageHasCheeps(_currentPage);
        
        return Page();
    }
    
    public async Task<IActionResult> OnPost()
    {
        var message = CheepInput.Message;
        if (string.IsNullOrWhiteSpace(message))
        {
            ModelState.AddModelError("Message", "Message cannot be empty.");
        }
        else if (message.Length > 160)
        {
            ModelState.AddModelError("Message", "Message cannot be more 160 characters.");
        }
        if (!ModelState.IsValid)
        {
            CheepTimelineModel.Cheeps = await GetCheeps(_currentPage);
            await FetchAuthorData();
            return Page();
        }

        await _cheepRepository.CreateCheep(User.Identity.Name, message);
        return Redirect("/");
    }

    public async Task<IActionResult> OnPostFollow(string authorName, int? page)
    {
        if (User.Identity.Name == authorName)
        {
            ModelState.AddModelError(string.Empty, "You cannot follow yourself.");
            return Redirect($"/?page={page}");
        }
        
        await _authorRepository.Follow(User.Identity.Name, authorName);
        CheepTimelineModel.FollowerMap[authorName] = true;
            
        return Redirect($"/?page={page}");
    }
    
    public async Task<IActionResult> OnPostUnfollow(string authorName, int? page)
    {
        await _authorRepository.Unfollow(User.Identity.Name, authorName);
        CheepTimelineModel.FollowerMap[authorName] = false;

        return Redirect($"/?page={page}");
    }
    
    public async Task<IActionResult> OnPostDelete(string cheepId, string author)
    {
        if (author != User.Identity.Name)
        {
            ModelState.AddModelError(cheepId, "Cannot delete cheep.");
            return RedirectToPage("Public");
        }
        
        try
        {
            await _cheepRepository.DeleteCheep(cheepId, User.Identity.Name);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine("Unable to delete cheep. Error: " + e.Message);
        }
        
        return RedirectToPage("Public");
    }
    
    public async Task<IActionResult> OnPostToggleLike(string cheepId, int? page)
    {
        var isLiked = await _cheepRepository.IsLiked(cheepId, User.Identity.Name);
        if (isLiked)
        {
            await _cheepRepository.Unlike(cheepId, User.Identity.Name);
            CheepTimelineModel.LikeMap[cheepId] = false;
        }
        else
        {
            await _cheepRepository.Like(cheepId, User.Identity.Name);
            CheepTimelineModel.LikeMap[cheepId] = true;
        }

        return Redirect($"/?page={page}");
    }

    private async Task<List<CheepDTO>> GetCheeps(int page)
    {
         return await _cheepRepository.GetCheepsByNewest(page);
    }
    
    private async Task FetchAuthorData()
    {
        foreach (var cheep in CheepTimelineModel.Cheeps)
        {
            if (cheep.Author != User.Identity.Name)
            {
                if (User.Identity.IsAuthenticated)
                {
                    CheepTimelineModel.FollowerMap[cheep.Author] = await _authorRepository.IsFollowing(User.Identity.Name, cheep.Author);
                    CheepTimelineModel.LikeMap[cheep.Id] = await _cheepRepository.IsLiked(cheep.Id, User.Identity.Name);
                }
            }
            CheepTimelineModel.AvatarMap[cheep.Author] = await _authorRepository.GetProfilePicture(cheep.Author);
        }
    }

    private async Task<bool> NextPageHasCheeps(int page)
    {
        var list = await _cheepRepository.GetCheeps(page + 1);
        return list.Any();
    }
    
}
