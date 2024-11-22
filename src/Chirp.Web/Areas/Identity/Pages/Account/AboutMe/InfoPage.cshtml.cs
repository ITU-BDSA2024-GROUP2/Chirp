using System.Collections;
using Chirp.Core;
using Chirp.Web.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Areas.Identity.Pages.Account.AboutMe
{
    public class InfoPage : PageModel
    {
        private readonly ICheepRepository _cheepRepository;
        private readonly IAuthorRepository _authorRepository;

        public Dictionary<string, bool> FollowerMap;
        private int _currentPage;
        public List<CheepDTO> Cheeps { get; set; }

        public ICollection<Author> following { get; set; }
        public ICollection<Author> followers { get; set; }

        private readonly UserManager<Author> _userManager;
        public string LoginProvider { get; set; } = "Default";

        
        
        public InfoPage(UserManager<Author> userManager, ICheepRepository cheepRepository, IAuthorRepository authorRepository)
        {
            _cheepRepository = cheepRepository;
            _authorRepository = authorRepository;
            
            
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet([FromQuery] int? page)
        {
            //check if there is a user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Retrieve external logins for the user
            var logins = await _userManager.GetLoginsAsync(user);
            if (logins.Any())
            {
                var externalLogin = logins.First();
                LoginProvider = externalLogin.LoginProvider;
            }
            
            _currentPage = page ?? 1;
            
            // Get Author from username
            var username = User.Identity!.Name!;
            var author = await _authorRepository.FindAuthor(username);
            
            Cheeps = await GetAuthorCheeps(username, _currentPage);
            following = author.Following;
            followers = author.Followers;
            

            return Page();
        }
        
        private async Task<List<CheepDTO>> GetAuthorCheeps(string author, int page)
        {
            Cheeps = await _cheepRepository.GetCheepsFromAuthor(author, page);
            return Cheeps;
        }
        
        
        
    }
}