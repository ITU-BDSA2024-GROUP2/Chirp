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
        var newCheep = Cheep.CreateCheep("Hello from Web Service!");
        
        // Act
        // Send a POST request to /cheep to store the cheep
        HttpResponseMessage postResponse = await Client.PostAsJsonAsync("/cheep", newCheep);
        postResponse.EnsureSuccessStatusCode();

        // Send a GET request to /cheeps to retrieve the stored cheeps
        var cheeps = await Client.GetFromJsonAsync<List<Cheep>>("/cheeps");

        // Assert
        Assert.NotNull(cheeps);
        Assert.Contains(cheeps, c => c.Message == "Hello from Web Service!" && c.Author == Environment.UserName);
    }
    
    
    
    
    
    
    
    
    
    
    
    /*
     private readonly string directoryPath = "../../../../../src/Chirp.CLI";
     
    private readonly string csvPath = "../../../../../data/chirp_cli_db.csv";
    
    [Fact]
    public void CSVDB_cheepCommand()
    {
        string command = "cheep Hello!!!";
        
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = @$"{directoryPath}",
                Arguments = $"run {command}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        CSVDatabase<Cheep>.Instance.SetFilePath(csvPath);
        var cheeps = CSVDatabase<Cheep>.Instance.Read();
        var lastCheep = cheeps.LastOrDefault(); 

        Assert.NotNull(lastCheep);
        Assert.Equal("Hello!!!", lastCheep.Message);
        Assert.Equal(Environment.UserName, lastCheep.Author);
    }
    */
}