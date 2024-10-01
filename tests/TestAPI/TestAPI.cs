using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

public class TestAPI : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public TestAPI(WebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true, HandleCookies = true });
    }

    [Fact]
    public async void CanSeePublicTimeline()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public async void CanSeePrivateTimeline(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }
    
    [Fact]
    public async void IsPage1SameAsDefaultTimeline()
    {
        var responseHomePage = await _client.GetAsync("/");
        var content1 = await responseHomePage.Content.ReadAsStringAsync();
        responseHomePage.EnsureSuccessStatusCode();
        var responseFirstPage = await _client.GetAsync("/?page=1");
        var content2 = await responseFirstPage.Content.ReadAsStringAsync();
        responseFirstPage.EnsureSuccessStatusCode();
        
        Assert.Contains("Chirp!", content1);
        Assert.Contains("Public Timeline", content1);
        Assert.Contains("Chirp!", content2);
        Assert.Contains("Public Timeline", content2);
        Assert.Equal(content1, content2);
    }
    
    [Fact]
    public async void SiteHasMorePagesAndNotSameAsSameAsDefaultTimeline()
    {
        var responseHomePage = await _client.GetAsync("/");
        var content1 = await responseHomePage.Content.ReadAsStringAsync();
        responseHomePage.EnsureSuccessStatusCode();
        
        var responsePage4 = await _client.GetAsync("/?page=4");
        responsePage4.EnsureSuccessStatusCode();
        var content2 = await responsePage4.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content1);
        Assert.Contains("Public Timeline", content1);
        Assert.Contains("Chirp!", content2);
        Assert.Contains("Public Timeline", content2);
        Assert.NotEqual(content1, content2);
    }
    
    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public async void PrivateTimelinePage1SameAsPrivateDefault(string author)
    {
        var responseHomePage = await _client.GetAsync($"/{author}");
        var content1 = await responseHomePage.Content.ReadAsStringAsync();
        responseHomePage.EnsureSuccessStatusCode();
        
        var responsePage4 = await _client.GetAsync($"/{author}?page=1");
        responsePage4.EnsureSuccessStatusCode();
        var content2 = await responsePage4.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content1);
        Assert.Contains($"{author}'s Timeline", content1);
        Assert.Contains("Chirp!", content2);
        Assert.Contains($"{author}'s Timeline", content2);
        Assert.Equal(content1, content2);
    }
    
    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public async void PrivateTimelinePage4NotSameAsPrivateDefault(string author)
    {
        var responseHomePage = await _client.GetAsync($"/{author}");
        var content1 = await responseHomePage.Content.ReadAsStringAsync();
        responseHomePage.EnsureSuccessStatusCode();
        
        var responsePage4 = await _client.GetAsync($"/{author}?page=4");
        responsePage4.EnsureSuccessStatusCode();
        var content2 = await responsePage4.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content1);
        Assert.Contains($"{author}'s Timeline", content1);
        Assert.Contains("Chirp!", content2);
        Assert.Contains($"{author}'s Timeline", content2);
        Assert.NotEqual(content1, content2);
    }
}