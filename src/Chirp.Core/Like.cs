namespace Chirp.Core;

public class Like
{
    public string LikeId { get; set; } = new Guid().ToString();
    public string Author { get; set; }
    public string CheepId { get; set; }
}