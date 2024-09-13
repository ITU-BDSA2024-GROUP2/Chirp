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
        
   
        [Fact]
        public void PrintingCheepsTest()
        {
            //Arrange
            Cheep Cheep1 = new Cheep("Michael", "I have a ball", 1690891760);
            Cheep Cheep2 = new Cheep("Poppy", "My balls are gone", 1690978778);
            Cheep Cheep3 = new Cheep("Sam", "I took Poppy's balls :)", 1690979858);
            //Act
            //string result = Cheep1.ToString();
            //IEnumerable<Cheep> cheeps = new IEnumerable<>();
            //cheeps.append(Cheep1);
            // cheeps.append(Cheep2);
            // cheeps.append(Cheep3);
            //UserInterface.printCheeps(cheeps);
            //Assert
            string expectedResult = "Michael @ 01/08/23 14:09:20: I have a ball";
            //Assert.Equal(expectedResult, _consoleOutput.ToString()); 
        }

        //Database Read and Store
       /* [Fact]
        public void SavingAndReadingCheepsInDatabaseTest()
        {
            //Arrange
            Cheep cheep1 = new Cheep("Michael", "I have a ball", 1690891760);
            Cheep cheep2 = new Cheep("Poppy", "My balls are gone", 1690978778);
            Cheep cheep3 = new Cheep("Sam", "I took Poppy's balls :)", 1690979858);
            private static IDatabaseRepository<Cheep> databaseTest = new CSVDatabase<Cheep>();

            //Act
            databaseTest.Store(cheep1);
            databaseTest.Store(cheep2);
            databaseTest.Store(cheep3);
      
            List<Cheep> cheepsTest = database.Read(3).ToList();
  
            //Assert
            Assert.Equal(Cheep1, cheepsTest[0]);
            //Assert.Equal(Cheep2, cheepsTest[1]);
            //Assert.Equal(Cheep3, cheepsTest[2]);
        }*/
    
    }
}