namespace Chirp.Core;

public class Like
{
    public string LikeId { get; set; } = Guid.NewGuid().ToString();
    public string? AuthorId { get; set; }
    public string? CheepId { get; set; }
}