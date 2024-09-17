namespace DefaultNamespace;

public class Chirp_CLI_End2endTests
{
    
    [Fact]
    public void CsvToCheepInConsole() //tror den er e2e
    {
        // Arrange
        string path = "../../../../../data/CsvParseTest.csv";
        
        // Act
        var cheeps = CSVParser.Parse<Cheep>(path);
        using (var consoleOutput = new StringWriter())
        {
            Console.SetOut(consoleOutput);


            // Act
            UserInterface.PrintCheeps(cheeps);
            var outputLines = consoleOutput.ToString().Trim().Split(Environment.NewLine); //source: https://stackoverflow.com/a/22878533 .newLine sikrer at det virker både på mac og windows
            

            // Assert
            Assert.Equal("ageh @ 01/08/23 14:09:20: SIIIIIUUUUUUUU!", outputLines[0].Trim());
            Assert.Equal("nitn @ 02/08/23 14:19:38: Recently engaged", outputLines[1].Trim());
        }
    }
}