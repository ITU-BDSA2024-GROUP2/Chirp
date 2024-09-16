using System.Reflection;
using System;
using Chirp.CLI;

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

}
