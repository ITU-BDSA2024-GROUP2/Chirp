using Chirp.CLI;
using CSVDB;
using System.Diagnostics;

public class Chirp_CLI_End2endTests
{
    [Fact]
    public void CSVDB_readCommand()
    {
        string command = "read 5";
        string path = "../../../../../src/Chirp.CLI";

        string expectedOutput = "ropf @ 01/08/23 14:09:20: Hello, BDSA students!" +
                                "\nadho @ 02/08/23 14:19:38: Welcome to the course!" +
                                "\nadho @ 02/08/23 14:37:38: I hope you had a good summer." +
                                "\nropf @ 02/08/23 15:04:47: Cheeping cheeps on Chirp :)\n";
        
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = @$"{path}",
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
}