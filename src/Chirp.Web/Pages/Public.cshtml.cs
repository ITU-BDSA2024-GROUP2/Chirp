#nullable disable //fjern null warning
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Azure.Identity;
using Chirp.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    public Dictionary<string, bool> FollowerMap;
    public Dictionary<string, bool> LikeMap;
    public int _currentPage;
    
    public List<CheepDTO> Cheeps { get; set; }
    
    [BindProperty]
    public CheepInputModel CheepInput { get; set; }
    
    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        FollowerMap = new Dictionary<string, bool>();
        LikeMap = new Dictionary<string, bool>();

    }

    public async Task<ActionResult> OnGet([FromQuery] int? page)
    {
        _currentPage = page ?? 1;
        ViewData["CurrentPage"] = _currentPage;
        
        await FetchCheepAndAuthorData(_currentPage);
        
        Console.WriteLine(LikeMap.Count);
        Console.WriteLine(FollowerMap.Count);
        
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
            await FetchCheepAndAuthorData(_currentPage);
            return Page();
        }

        await _cheepRepository.CreateCheep(User.Identity.Name, CheepInput.Message);
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
        FollowerMap[authorName] = true;
            
        return Redirect($"/?page={page}");
    }
    
    public async Task<IActionResult> OnPostUnfollow(string authorName, int? page)
    {
        await _authorRepository.Unfollow(User.Identity.Name, authorName);
        
        FollowerMap[authorName] = false;

        return Redirect($"/?page={page}");
    }
    
    public async Task<IActionResult> OnPostDelete(string cheepId)
    {
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
    
    public async Task<IActionResult> OnPostLike(string cheepId, int? page)
    {
        await _cheepRepository.Like(cheepId, User.Identity.Name);
        LikeMap[cheepId] = true;
        
        return Redirect($"/?page={page}");
    }
    
    public async Task<IActionResult> OnPostUnlike(string cheepId, int? page)
    {
        await _cheepRepository.Unlike(cheepId, User.Identity.Name);
        LikeMap[cheepId] = false;
        
        return Redirect($"/?page={page}");
    }
    
    private async Task FetchCheepAndAuthorData(int page)
    {
        Cheeps = await _cheepRepository.GetCheeps(page);

        if (User.Identity.IsAuthenticated)
        {
            foreach (var cheep in Cheeps)
            {
                if (cheep.Author != User.Identity.Name)
                {
                    FollowerMap[cheep.Author] = await _authorRepository.IsFollowing(User.Identity.Name, cheep.Author);
                    FollowerMap[cheep.Id] = await _cheepRepository.IsLiked(cheep.Id, User.Identity.Name);
                }
            }
        }
    }
}
