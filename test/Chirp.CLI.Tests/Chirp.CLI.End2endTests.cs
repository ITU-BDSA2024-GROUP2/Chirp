using Chirp.CLI;
using CSVDB;
using System.Diagnostics;
using System.Net.Http.Json;

public class Chirp_CLI_End2endTests
{
   
    private const string BaseUrl = "http://localhost:5282";
    private static readonly HttpClient Client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

    private readonly string csvPath = "../../../../../data/chirp_cli_db.csv";

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