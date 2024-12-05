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
}