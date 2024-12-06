using System.Security.Claims;
using Azure;
using Chirp.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class CheepTimelineModel
{
    public List<CheepDTO> Cheeps { get; set; }
    public Dictionary<string, bool> FollowerMap;
    public Dictionary<string, bool> LikeMap;
    public Dictionary<string, string?> AvatarMap;
    
    public CheepTimelineModel()
    {
        FollowerMap = new Dictionary<string, bool>();
        LikeMap = new Dictionary<string, bool>();
        AvatarMap = new Dictionary<string, string?>();
    }
    
    public async Task FetchAuthorData(ClaimsPrincipal user, IAuthorRepository authorRepository, ICheepRepository cheepRepository)
    {
        foreach (var cheep in Cheeps)
        {
            if (cheep.Author != user.Identity.Name)
            {
                if (user.Identity.IsAuthenticated)
                {
                    FollowerMap[cheep.Author] = await authorRepository.IsFollowing(user.Identity.Name, cheep.Author);
                    LikeMap[cheep.Id] = await cheepRepository.IsLiked(cheep.Id, user.Identity.Name);
                }
            }
            AvatarMap[cheep.Author] = await authorRepository.GetProfilePicture(cheep.Author);
        }
    }
}