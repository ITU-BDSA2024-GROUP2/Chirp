using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

public class Author : IdentityUser
{
    public ICollection<Cheep>? Cheeps { get; set; } = new List<Cheep>();
    
    public ICollection<Author> Following { get; set; } = new List<Author>();
    
    public ICollection<Author> Followers { get; set; } = new List<Author>();
}