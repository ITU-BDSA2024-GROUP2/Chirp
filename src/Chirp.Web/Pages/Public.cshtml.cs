#nullable disable //fjern null warning
using System.ComponentModel.DataAnnotations;
using Azure.Identity;
using Chirp.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorRepository _authorRepository;
    public Dictionary<string, bool> FollowerMap;
    
    public List<CheepDTO> Cheeps { get; set; }
    
    [BindProperty]
    public CheepViewModel CheepInput { get; set; }
    
    public PublicModel(ICheepService cheepService, IAuthorRepository authorRepository)
    {
        _cheepService = cheepService;
        _authorRepository = authorRepository;
        FollowerMap = new Dictionary<string, bool>();

    }

    public async Task<ActionResult> OnGet([FromQuery] int? page)
    {
        var currentPage = page ?? 1;
        
        Cheeps = await _cheepService.GetCheeps(currentPage);
        
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
            Cheeps = await _cheepService.GetCheeps(1);
            return Page();
        }

        await _cheepService.CreateCheep(User.Identity.Name, CheepInput.Message);
        return RedirectToPage("Public");
    }

    public async Task<IActionResult> OnPostFollow(string authorName)
    {
        await _authorRepository.Follow(User.Identity.Name, authorName);
        
        FollowerMap[authorName] = true;
        return RedirectToPage("Public");
    }
    
    public async Task<IActionResult> OnPostUnfollow(string authorName)
    {
        await _authorRepository.Unfollow(User.Identity.Name, authorName);
        
        FollowerMap[authorName] = false;
        return RedirectToPage("Public");
    }

    public async Task<bool> IsFollowing(string userName, string authorName)
    {
        return await _authorRepository.IsFollowing(userName, authorName);
    }
    
}
