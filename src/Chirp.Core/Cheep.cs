using System.ComponentModel.DataAnnotations;

namespace Chirp.Core;

public class Cheep
{
    public int CheepId { get; set; }
    [Required]
    [StringLength(160)]
    public string? Text { get; set; }
    public DateTime TimeStamp { get; set; }
    [Required]
    public Author? Author { get; set; }
    
    public int AuthorId { get; set; }
}