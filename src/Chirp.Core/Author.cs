using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core;

public class Author : IdentityUser
{
    [PersonalData]
    [StringLength(64)]
    public string? Name { get; set; }
    public ICollection<Cheep>? Cheeps { get; set; } = new List<Cheep>();
}