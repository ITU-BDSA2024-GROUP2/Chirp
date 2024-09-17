using Chirp.CLI;
using CSVDB;
using System.Diagnostics;

public class Chirp_CLI_End2endTests
{
    [Theory]
    [InlineData("read 10", 
        "ropf @ 01/08/23 14:09:20: Hello, BDSA students!\n" +
        "adho @ 02/08/23 14:19:38: Welcome to the course!\n" +
        "adho @ 02/08/23 14:37:38: I hope you had a good summer.\n" +
        "ropf @ 02/08/23 15:04:47: Cheeping cheeps on Chirp :)")]
    public void CSVDB_readCommand(string arguments, string expectedOutput)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = @"../../../../../src/Chirp.CLI",
                Arguments = $"run {arguments}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        
        var normalizedActualOutput = NormalizeOutput(output);
        var normalizedExpectedOutput = NormalizeOutput(expectedOutput);
        
        Assert.Equal(normalizedExpectedOutput, normalizedActualOutput);
    }
    
    private string NormalizeOutput(string output)
    {
        using var reader = new StringReader(output);
        using var writer = new StringWriter();
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            writer.WriteLine(line.Trim());  // Trim each line
        }
        return writer.ToString().Trim();  // Trim the final result
    }
}