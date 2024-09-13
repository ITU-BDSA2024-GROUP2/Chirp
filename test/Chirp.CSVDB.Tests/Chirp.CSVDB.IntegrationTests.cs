using Xunit;

/* Naming your tests
   The name of your test should consist of three parts:

   The name of the method being tested.
   The scenario under which it's being tested.
   The expected behavior when the scenario is invoked.

    // Arrange - Define all the parameters and create an instance of the system (class) under test (SUT).

    // Act - Execute the method being tested, and capture the result.

    // Assert - Verify that the result of the Act stage had the expected value.
*/

namespace Chirp.CLI.UnitTests
{
    public class Chirp_CSVDB_IntegrationTests
    {
        [Fact]
        public void CSVDB_StoreAndReadCheep()
        {
            // Arrange
            string Author = "John Smith";
            string Message = "Hello, world!";
            long Timestamp = 123456789;
            
            Cheep cheep = new Cheep(Author, Message, Timestamp);
            CSVDatabase<T> csvDatabase = new CSVDatabase<>(T);
            
            // Act
            csvDatabase.Store(cheep);
            
            // Assert
            
            var cheeps = CSVParser.Parse<Cheep>();
            var storedCheep = cheeps.FirstOrDefault();
            
            Assert.NotNull(storedCheep);
            Assert.Equal("John Smith", storedCheep.Author);
            Assert.Equal("Hello, world!", storedCheep.Message);
            Assert.Equal(123456789, storedCheep.Timestamp);
            
        }
    }
}