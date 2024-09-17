
using Chirp.CLI;
using CSVDB;

public class Chirp_CSVDB_IntegrationTests
{
    private readonly string testCsvFilePath;

    public Chirp_CSVDB_IntegrationTests()
    {
        testCsvFilePath = Path.GetTempFileName();
        
        using (var writer = new StreamWriter(testCsvFilePath))
        {
            // Write header
            writer.WriteLine("Author,Message,Timestamp");
        }
        
        CSVDatabase<Cheep>.Instance.SetFilePath(testCsvFilePath);
    }
    
    [Fact]
    public void CSVDB_StoreAndReadCheep()
    {
        // Arrange
        string Author = "John Smith";
        string Message = "Hello, world!";
        long Timestamp = 123456789;
            
        Cheep cheep = new Cheep(Author, Message, Timestamp);
            
        // Act
        CSVDatabase<Cheep>.Instance.Store(cheep);
            
        // Assert
        var cheeps = CSVDatabase<Cheep>.Instance.Read();
        var storedCheep = cheeps.FirstOrDefault();
        
        //Console.WriteLine($"Author: {Author}, Message: {Message}, Timestamp: {Timestamp}");
            
        Assert.NotNull(storedCheep);
        Assert.Equal("John Smith", storedCheep.Author);
        Assert.Equal("Hello, world!", storedCheep.Message);
        Assert.Equal(123456789, storedCheep.Timestamp);
    }
    
    public void Dispose()
    {
        // Cleanup: Delete the temporary file after each test
        if (File.Exists(testCsvFilePath))
        {
            File.Delete(testCsvFilePath);
        }
    }
}