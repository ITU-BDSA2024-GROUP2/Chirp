using Chirp.Core;

namespace Chirp.Infrastructure;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    
    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }
    
    public async Task<List<Author>> GetFollowingAuthors(string authorName)
    {
        var following = await _authorRepository.GetFollowingAuthors(authorName);
        return following;
    }
    
}