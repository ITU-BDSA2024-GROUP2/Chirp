#nullable disable
using Chirp.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages
{
    public class About : PageModel
    {
        private readonly ICheepRepository _cheepRepository;
        private readonly IAuthorRepository _authorRepository;
        
        public List<CheepDTO> Cheeps { get; set; }
        public ICollection<string> Following { get; set; }
        public ICollection<string> Followers { get; set; }
        public Dictionary<string, string> UserInfo { get; set; }
        
        public string? ID { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? GithubURL { get; set; }
        public string? Avatar { get; set; }

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
            try
            {
                await _cheepRepository.DeleteCheep(cheepId, User.Identity.Name);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Unable to delete cheep. Error: " + e.Message);
            }
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
            
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }
            
            Username = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            Email = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            Name = User.FindFirst("urn:github:name")?.Value;
            GithubURL = User.FindFirst("urn:github:url")?.Value;
            Avatar = $"https://avatars.githubusercontent.com/{Username}";
        }
    }
}