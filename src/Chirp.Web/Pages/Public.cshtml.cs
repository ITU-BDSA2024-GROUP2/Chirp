#nullable disable //fjern null warning
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Azure.Identity;
using Chirp.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    public Dictionary<string, bool> FollowerMap;
    public int _currentPage;
    
    public List<CheepDTO> Cheeps { get; set; }
    
    [BindProperty]
    public CheepInputModel CheepInput { get; set; }
    
    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        FollowerMap = new Dictionary<string, bool>();

    }

    public async Task<ActionResult> OnGet([FromQuery] int? page)
    {
        _currentPage = page ?? 1;
        ViewData["CurrentPage"] = _currentPage;
        
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
        return Redirect("Public");
    }

    public async Task<IActionResult> OnPostFollow(string authorName, int? page)
    {
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
        await _cheepRepository.DeleteCheep(cheepId);
        return RedirectToPage("Public");
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
