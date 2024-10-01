using System.Reflection;
using Chirp.Razor;
using Microsoft.Extensions.FileProviders;

public class DBFacadeTests
{
    private DBFacade _dbFacade;
    
    public DBFacadeTests() {
        Environment.SetEnvironmentVariable("CHIRPDBPATH", "Data Source=:memory:"); // Source: https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#customize-webapplicationfactory
    }

    [Fact]
    public void InitDBTest()
    {
        // Arrange
        Environment.SetEnvironmentVariable("CHIRPDBPATH", "readCheepsTestData");
        _dbFacade = new DBFacade();

        // Act
        var cheeps = _dbFacade.ReadCheeps(1, 10);
        
        CheepViewModel cheep = CheepViewModel.CreateCheep("Hej", "test", 1213213123);
        
        // Assert
        Assert.NotNull(cheeps);
        Assert.Empty(cheeps); // assuming no data is pre-populated
    }
    
    [Fact]
    public void ReadCheepsTest()
    {
        // Arrange
        _dbFacade = new DBFacade();
        PopulateDbWithTestData();
        
        // Act
        var cheeps = _dbFacade.ReadCheeps(1, 10);
        Assert.NotNull(cheeps);
        Assert.Equal(2, cheeps.Count);
        
        // Assert
        Assert.Equal("TestUser1", cheeps[1].Author);
        Assert.Equal("Test cheep 1", cheeps[1].Message);
        Assert.Equal("09.29.23 20.58.14", cheeps[1].Timestamp);
        
        Assert.Equal("TestUser2", cheeps[0].Author);
        Assert.Equal("Test cheep 2", cheeps[0].Message);
        Assert.Equal("09.29.23 20.59.54", cheeps[0].Timestamp);
    }

    [Theory]
    [InlineData("TestUser1", 1, 10)]
    [InlineData("TestUser2", 1, 10)]
    public void ReadCheepsFromAuthor_Returns_Cheeps_For_Existing_Author(string author, int pageNumber, int pageSize)
    {
        // Arrange
        _dbFacade = new DBFacade();
        PopulateDbWithTestData();
        
        // Act
        var cheeps = _dbFacade.ReadCheepsFromAuthor(author,1, 10);
        Assert.NotNull(cheeps);
        Assert.Single(cheeps);
        
        // Assert
        Assert.Equal(author, cheeps[0].Author);
    }
    
    [Fact]
    public void ReadCheepsFromAuthor_Returns_Empty_For_NonExistent_Author()
    {
        // Arrange
        _dbFacade = new DBFacade();
        PopulateDbWithTestData();
        
        // Act
        var result = _dbFacade.ReadCheepsFromAuthor("NonExistentAuthor", 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    private void PopulateDbWithTestData()
    {
        //Creates schema from embedded file
        var embeddedProvider = new EmbeddedFileProvider(Assembly.Load("Chirp.Razor"));
        
        using var schemaReader = embeddedProvider.GetFileInfo("data/schema.sql").CreateReadStream();
        using var schemaStreamReader = new StreamReader(schemaReader);
        var schema = schemaStreamReader.ReadToEnd();
        _dbFacade.ExecuteNonQuery(schema);

        // Insert test users
        var insertUserSql = @"
            INSERT INTO user VALUES (1, 'TestUser1', 'TestUser1@itu.dk', 'password');
            INSERT INTO user VALUES (2, 'TestUser2', 'TestUser2@itu.dk', 'password');";
        _dbFacade.ExecuteNonQuery(insertUserSql);

        // Insert test messages
        var insertMessageSql = @"
            INSERT INTO message (author_id, text, pub_date) VALUES (1, 'Test cheep 1', 1696021094);
            INSERT INTO message (author_id, text, pub_date) VALUES (2, 'Test cheep 2', 1696021194);";
        _dbFacade.ExecuteNonQuery(insertMessageSql);
    }
}