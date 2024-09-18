using Chirp.CLI;
using CSVDB;
using System.Diagnostics;

public class Chirp_CLI_End2endTests
{
    private readonly string directoryPath = "../../../../../src/Chirp.CLI";
    private readonly string csvPath = "../../../../../data/chirp_cli_db.csv";
    
    [Fact]
    public void CSVDB_readCommand()
    {
        string command = "read 5";

        string expectedOutput = "ropf @ 01/08/23 14:09:20: Hello, BDSA students!" +
                                "\nadho @ 02/08/23 14:19:38: Welcome to the course!" +
                                "\nadho @ 02/08/23 14:37:38: I hope you had a good summer." +
                                "\nropf @ 02/08/23 15:04:47: Cheeping cheeps on Chirp :)\n";
        
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

        Assert.Equal(expectedOutput, output);
    }

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
        Assert.NotNull(lastCheep.Timestamp);
    }
}