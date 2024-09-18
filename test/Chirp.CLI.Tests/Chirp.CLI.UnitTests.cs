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
        
        
    [Fact]
    public void CsvParserTest()
    {
        // Arrange
        string path = "../../../../../data/CsvParseTest.csv";

        // Act
        var cheeps = CSVParser.Parse<Cheep>(path);
        var cheep1 = cheeps.FirstOrDefault();
        
        // Assert
        Assert.Equal("ageh", cheep1.Author);
        Assert.Equal("SIIIIIUUUUUUUU!", cheep1.Message);
        Assert.Equal(1690891760, cheep1.Timestamp);
    }
        
}
