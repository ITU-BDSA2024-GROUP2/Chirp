using System.ComponentModel.DataAnnotations;

namespace Chirp.Razor;

public class Author
{
    public int AuthorId { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? Email { get; set; }
    public ICollection<Cheep>? Cheeps { get; set; }
}