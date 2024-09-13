using Xunit;
using System.Reflection;
using System;

/* Naming your tests
   The name of your test should consist of three parts:
   
   The name of the method being tested.
   The scenario under which it's being tested.
   The expected behavior when the scenario is invoked.
*/

namespace Chirp.CLI.UnitTests
{
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
        [InlineData("Michael", "I have a ball", 1690891760, 
            "Michael @ 01/08/23 14:09:20: I have a ball")]
        [InlineData("Poppy", "My balls are gone", 1690978778,
            "Poppy @ 02/08/23 14:19:38: My balls are gone")]
        [InlineData("Sam", "I took Poppy's balls :)", 1690979858,
            "Sam @ 02/08/23 14:37:38: I took Poppy's balls :)")]
        public void CheepToStringTest(string author, string message, long timeStamp, string expectedResult)
        {
            //Arrange
            Cheep cheep = new Cheep(author, message, timeStamp);
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
}