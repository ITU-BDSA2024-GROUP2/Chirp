using System.Collections;
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
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }
            
            UserInfo = new Dictionary<string, string>();
            await LoadUserInfo();
            
            return Page();
        }
        
        public async Task<IActionResult> OnPostDelete(string cheepId)
        {
            await _cheepRepository.DeleteCheep(cheepId);
            return RedirectToPage("About");
        }

        private async Task LoadUserInfo()
        {
            var username = User.Identity?.Name;
            
            // Username
            UserInfo.Add("username", username);
            
            // Email
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var email = await _userManager.GetEmailAsync(user);
                UserInfo.Add("email", email ?? "not registered");
            }

            // Login
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
            foreach (var follower in Followers)
            {
                Console.WriteLine(username + " follows " + follower);
            }
            UserInfo.Add("followerCount", Followers.Count.ToString());
            
            // Following
            Following = await _authorRepository.GetFollowing(username);
            UserInfo.Add("followingCount", Following.Count.ToString());
        }
    }
}