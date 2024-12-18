namespace Chirp.Core;

/// <summary>
/// This class repesents likes on a cheep.
/// An author can only like the same cheep once.
/// An author can also remove their like on a liked cheep.
/// </summary>
public class Like
{
    public string LikeId { get; set; } = Guid.NewGuid().ToString();
    public string? AuthorId { get; set; }
    public string? CheepId { get; set; }
}