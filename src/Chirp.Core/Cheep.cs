using System.ComponentModel.DataAnnotations;

namespace Chirp.Core;

public class Cheep
{
    public int CheepId { get; set; }
    [Required]
    [StringLength(160, ErrorMessage = "Cheeps can't be longer than 160 characters.")]
    public string? Text { get; set; }
    public DateTime TimeStamp { get; set; }
    [Required]
    public Author? Author { get; set; }
}