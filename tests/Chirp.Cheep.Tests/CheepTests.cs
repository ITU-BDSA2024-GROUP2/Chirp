using Chirp.Razor;

public class CheepViewModelTests {
    [Theory]
    [InlineData("Michael", "I have a ball", 1690891760, "Michael @ 08/01/23 12:09:20: I have a ball")]
    [InlineData("Poppy", "My balls are gone", 1690978778, "Poppy @ 08/02/23 12:19:38: My balls are gone")]
    public void CheepToStringTest(string author, string message, long timeStamp, string expectedResult)
    {
        //Arrange
        CheepViewModel cheep = CheepViewModel.CreateCheep(author, message, timeStamp);
        
        //Act
        string result = cheep.ToString();
        
        //Assert
        Assert.Equal(expectedResult, result);
    }
}