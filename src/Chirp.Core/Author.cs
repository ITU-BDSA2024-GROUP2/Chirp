using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

/// <summary>
/// This class repesents the user of the Chirp application.
/// An author can post cheeps. An author can follow another or more authors.
/// An author knows its followers. 
/// </summary>
public class Author : IdentityUser
{
    public ICollection<Cheep>? Cheeps { get; set; } = new List<Cheep>();
    
    public ICollection<Author> Following { get; set; } = new List<Author>();
    
    public ICollection<Author> Followers { get; set; } = new List<Author>();

    [StringLength(1600, ErrorMessage = "links cant be longer than 1600")]
    
    public string? ProfilePicture { get; set; }

}