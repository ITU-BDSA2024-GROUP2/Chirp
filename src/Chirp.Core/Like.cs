namespace Chirp.Core;

/// <summary>
/// This class repesents likes on a cheep.
/// A user can only like the same cheep once.
/// A user can also remove their on a liked cheep.
/// </summary>
public class Like
{
    public string LikeId { get; set; } = Guid.NewGuid().ToString();
    public string? AuthorId { get; set; }
    public string? CheepId { get; set; }
}