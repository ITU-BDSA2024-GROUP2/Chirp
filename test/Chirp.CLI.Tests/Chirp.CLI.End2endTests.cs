using Chirp.CLI;
using CSVDB;
using System.Diagnostics;
using System.Net.Http.Json;

public class Chirp_CLI_End2endTests
{
   
    private const string BaseUrl = "https://bdsagroup2chirpremotedb-eqdfdrh6hrdbceer.northeurope-01.azurewebsites.net/";
    private static readonly HttpClient Client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

    [Fact]
    public async Task WebService_CheepCommand()
    {
        // Arrange
        var newCheep = Cheep.CreateCheep("I am a TEST!!");
        
        // Act
        HttpResponseMessage postResponse = await Client.PostAsJsonAsync("/cheep", newCheep);
        
        var cheeps = await Client.GetFromJsonAsync<List<Cheep>>("/cheeps");

        // Assert
        Assert.NotNull(cheeps);
        Assert.Contains(cheeps, c => c.Message == "I am a TEST!!" && c.Author == Environment.UserName);
        Assert.True(postResponse.IsSuccessStatusCode);
        
    }
}