using System.Reflection;
using System;
using Chirp.CLI;
using CSVDB;

/* Naming your tests
   The name of your test should consist of three parts:
   
   The name of the method being tested.
   The scenario under which it's being tested.
   The expected behavior when the scenario is invoked.
*/

public class Chirp_CLI_UnitTests
{
    [Fact]
    public void TestName()
    {
        // Arrange - Define all the parameters and create an instance of the system (class) under test (SUT).

        // Act - Execute the method being tested, and capture the result.

        // Assert - Verify that the result of the Act stage had the expected value.
		Assert.True(true);
	}
   
	[Theory]
    [InlineData("Michael", "I have a ball", 1690891760)]
    [InlineData("Poppy", "My balls are gone", 1690978778)]
    [InlineData("Sam", "I took Poppy's balls :)", 1690979858)]
    public void CheepToStringTest(string author, string message, long timeStamp)
    {
        //Arrange
        Cheep cheep = new Cheep(author, message, timeStamp);

		DateTime utcTime = DateTimeOffset.FromUnixTimeSeconds(timeStamp).UtcDateTime;
        DateTime localTime = utcTime.ToLocalTime();

        // Constructs the expected result based on the local time zone
        string expectedResult = $"{author} @ {localTime.ToString("dd'/'MM'/'yy HH':'mm':'ss")}: {message}";
		
        //Act
        string result = cheep.ToString();

        //Assert
        Assert.Equal(expectedResult, result);
    }
        
    [Theory]
    [InlineData("I am lost")]
    [InlineData("Where is the fridge?")]
    [InlineData("Do you even know what?")]
    [InlineData("Im a cool scooter")]
    public void CreateCheepFromMessageTest(string message)
    {
        //Arrange
        //Act
        Cheep cheep = Cheep.CreateCheep(message);
        //Assert
        Cheep expectedCheep = new Cheep(Environment.UserName, message, DateTimeOffset.Now.ToUnixTimeSeconds());
        Assert.Equal(expectedCheep, cheep);
    }
        
   
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
        
    [Fact]
    public void CsvParserTest()
    {
        // Arrange
        string path = "../../../../../data/CsvParseTest.csv";

        // Act
        var cheeps = CSVParser.Parse<Cheep>(path);
        var cheep1 = cheeps.FirstOrDefault();
        
        // Assert
        Assert.Equal(cheep1.Author, "ageh");
        Assert.Equal(cheep1.Message, "SIIIIIUUUUUUUU!");
        Assert.Equal(cheep1.Timestamp, 1690891760);
    }
        
        
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
