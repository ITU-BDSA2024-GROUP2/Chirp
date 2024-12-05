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
    public int cheepsLength;
    
    public CheepTimelineModel(UserManager<Author> userManager, ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _userManager = userManager;
        Cheeps = new List<CheepDTO>();
        cheepsLength = 0;
    }

    /// <summary>
    /// This method will get all the cheeps from the database. There is two of these methods,
    /// so that the public timeline and private timeline can retrieve the appropriate cheeps
    /// It retrieves the page from the web address,
    /// and gets the Avatar from the database.
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task GetCheeps(int page)
    {
        
        Cheeps = await _cheepRepository.GetCheepsByNewest(page);

        if (cheepsLength != Cheeps.Count())
        {
            await GetAvatar();
        }
        
    }
    
    /// <summary>
    /// This method retrieves the necessary cheeps from the database.
    /// This method is primarily used by the private timeline.
    /// It checks if the private timeline is the authors own page or someone else.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="author"></param>
    /// <returns></returns>
    public async Task GetCheeps(int page, string author, string username)
    {
        
        if (author == username)
        {
            Cheeps = await _cheepRepository.GetCheepsFromFollowersAndOwnCheeps(author, page);
        }
        else
        {
            Cheeps = await _cheepRepository.GetCheepsFromAuthor(author, page);
        }
        
        if (cheepsLength != Cheeps.Count())
        {
            await GetAvatar();
        }
    }

    /// <summary>
    /// Retrieves the Authors Avatar. Goes through each cheep, and finds the authors profile picture.
    /// </summary>
    public async Task GetAvatar()
    {
        foreach (var cheep in Cheeps)
        {
            var avatar = await _authorRepository.GetProfilePicture(cheep.Author);
            AuthorAvatars[cheep.Author] = avatar;
        }
    }
    
}