﻿using System.Collections;
using System.Security.Claims;
using Chirp.Core;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Areas.Identity.Pages.Account.AboutMe
{
    public class About : PageModel
    {
        private readonly ICheepRepository _cheepRepository;
        private readonly IAuthorRepository _authorRepository;
        
        public List<CheepDTO> Cheeps { get; set; }
        public ICollection<string> Following { get; set; }
        public ICollection<string> Followers { get; set; }
        public Dictionary<string, string> UserInfo { get; set; }

        private readonly UserManager<Author> _userManager;
        
        public About(UserManager<Author> userManager, ICheepRepository cheepRepository, IAuthorRepository authorRepository)
        {
            _cheepRepository = cheepRepository;
            _authorRepository = authorRepository;
            
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet([FromQuery] int? page)
        {
            UserInfo = new Dictionary<string, string>();
            await LoadUserInfo();
            
            return Page();
        }

        private async Task LoadUserInfo()
        {
            var username = User.Identity?.Name;
            var author = await _authorRepository.FindAuthor(username);
            
            // Username
            UserInfo.Add("username", username);
            
            // Email
            var email = User?.FindFirst(ClaimTypes.Email)?.Value;
            if (email != null)
            {
                UserInfo.Add("email", email);
            }
            
            // Login
            var user = await _userManager.FindByNameAsync(username);
            var logins = await  _userManager.GetLoginsAsync(user);
            if (logins.Any())
            {
                var externalLogin = logins.First();
                UserInfo.Add("login", externalLogin.LoginProvider);
            }
            else
            {
                UserInfo.Add("login", "default");
            }
            
            // Cheeps
            Cheeps = await _cheepRepository.GetAllCheepsFromAuthor(username);
            
            // Followers
            Followers = await _authorRepository.GetFollowers(username);
            UserInfo.Add("followerCount", Followers.Count.ToString());
            
            // Following
            Following = await _authorRepository.GetFollowing(username);
            UserInfo.Add("followingCount", Following.Count.ToString());
        }
    }
}