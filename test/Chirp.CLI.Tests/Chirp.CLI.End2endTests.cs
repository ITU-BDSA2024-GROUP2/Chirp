using Chirp.CLI;
using CSVDB;
using System.Diagnostics;

public class Chirp_CLI_End2endTests
{
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
}