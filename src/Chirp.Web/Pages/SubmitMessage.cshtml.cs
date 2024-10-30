using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class SubmitMessageModel : PageModel
{
    [BindProperty]
    [Required]
    [StringLength(250, ErrorMessage = "Maximum length is {1}")]
    [Display(Name = "Message Text")]
    public string Message { get; set; }
    
    public void OnGet()
    {
        
    }
}