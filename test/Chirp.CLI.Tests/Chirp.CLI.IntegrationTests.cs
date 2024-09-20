using Chirp.CLI;

public class Chirp_CLI_IntegrationTests
{
    [Theory]
    [InlineData("Michael", "I have a ball", 1690891760)]
    [InlineData("Harald", "I am him", 1726056884)]
    [InlineData("Anders", "I have Netflix", 1726056833)]
    public void PrintingCheepsTest(string author, string message, long timestamp)
    {
        // Arrange
        Cheep cheep = new Cheep(author, message, timestamp);
        List<Cheep> cheeps = new List<Cheep> { cheep };
        using (var consoleOutput = new StringWriter())
        {
            Console.SetOut(consoleOutput);

            // Act
            UserInterface.PrintCheeps(cheeps);
            string result = cheep.ToString();

            // Assert
            Assert.Equal(result, consoleOutput.ToString().Trim()); 
        }
    }
}