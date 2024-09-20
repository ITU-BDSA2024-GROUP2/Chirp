
using System.Net;
using System.Net.Http.Json;
using Chirp.CLI;

public class ChirpCSVDBServiceUnitTests
{
    private const string BaseUrl = "http://localhost:5282";

    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri(BaseUrl),
    };

    [Fact]
    public async Task GETCheeps_ReturnsStatusCode200AndListOfCheeps()
    {
        // Arrange
        var response = await Client.GetAsync("/cheeps");
        
        // Act
        var cheeps = await response.Content.ReadFromJsonAsync<List<Cheep>>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(cheeps);
        Assert.NotEmpty(cheeps);
    }
    
    [Fact]
    public async Task POSTCheep_ReturnsStatusCode200AndStoresCheep()
    {
        // Arrange
        var newCheep = Cheep.CreateCheep("Hello from CSVDBService Unit Test");
        
        // Act
        using var response = await Client.PostAsJsonAsync($"/cheep", newCheep);
        
        // Assert
    }
}