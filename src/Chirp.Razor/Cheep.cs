namespace Chirp.Razor;

public class Cheep
{
    public int CheepId { get; set; }
    public required string Text { get; set; }
    public DateTime TimeStamp { get; set; }
    public required Author Author { get; set; }
    
    public int AuthorId { get; set; }
}