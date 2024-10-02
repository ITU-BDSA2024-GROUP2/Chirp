namespace Chirp.Razor;

public class Author
{
    public int AuthorId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; }
}