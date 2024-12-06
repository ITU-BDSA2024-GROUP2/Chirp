using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Chirp.UI.Tests;

public class IntegrationTest : PageTest
{
    private Process _serverProcess;

    public override BrowserNewContextOptions ContextOptions()
    {

        return new BrowserNewContextOptions             
        {
            IgnoreHTTPSErrors = true
        };
    }

    [OneTimeSetUp]
    public async Task Setup()
    {
        _serverProcess = await ServerUtil.StartServer();
    }

    [OneTimeTearDown]
    public void Cleanup()
    {
        _serverProcess.Kill();
        _serverProcess.Dispose();
    }

    [TearDown]
    public async Task CleanupAccount()
    {
        await ServerUtil.DeleteUser(Page, "test@mail.com", "Testpassword123!");
        await ServerUtil.DeleteUser(Page, "testmail@mail.com", "Testpassword123!");
        await ServerUtil.DeleteUser(Page, "atest@mail.com", "Testpassword123!");
        await ServerUtil.DeleteUser(Page, "hello@mail.com", "Testpassword123!");
        await ServerUtil.DeleteUser(Page, "hello2@mail.com", "Testpassword123!");
    }
    
    [Test]
    public async Task UserRegistersAndTestsPagination()
    {
        //Arrange
        await ServerUtil.RegisterUser(Page, "ATestUser", "test@mail.com", "Testpassword123!");
            
        //Assert
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Previous Page" })).Not.ToBeVisibleAsync(); //On page 1, so should not be visible
            
        //Act
        await Page.GetByRole(AriaRole.Button, new() { Name = "Next Page" }).ClickAsync();
            
        //Assert
        Assert.That(Page.Url, Is.EqualTo("https://localhost:5273/?page=2"));
            
        //Act
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
            
        //Assert
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Previous Page" })).Not.ToBeVisibleAsync(); //On page 1, so should not be visible
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Next Page" })).Not.ToBeVisibleAsync(); //Havent posted or followed, so button should not be visible
            
        //Act
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the worst." }).Locator("#follow").ClickAsync(); //Follow jac.. because she has > 32 cheeps
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Next Page" }).ClickAsync();
            
        //Assert
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Previous Page" })).ToBeVisibleAsync(); 
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Next Page" })).ToBeVisibleAsync(); 
    }
    
    [Test]
    public async Task UserIsAbleToMakeAndDeleteCheeps()
    {
        //Arrange
        await ServerUtil.RegisterUser(Page, "ATestUser", "test@mail.com", "Testpassword123!");
            
        //Act
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync("This is a test cheep");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            
        await Page.GetByRole(AriaRole.Button, new() { Name = "DELETE" }).ClickAsync();
            
        //Assert
        await Expect(Page.GetByText("This is a test cheep")).Not.ToBeVisibleAsync();
            
        //Act
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
            
        //Assert
        await Expect(Page.GetByText("This is a test cheep")).Not.ToBeVisibleAsync();
            
        //Act
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync("This is a test cheep on my timeline");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            
        //Assert
        await Expect(Page.GetByText("This is a test cheep on my timeline")).ToBeVisibleAsync();
            
        //Act
        await Page.GetByRole(AriaRole.Button, new() { Name = "DELETE" }).ClickAsync();
            
        //Assert
        await Expect(Page.GetByText("This is a test cheep on my timeline")).Not.ToBeVisibleAsync();
    }
    
    [Test]
    public async Task UserChecksMyTimelineForOwnCheeps()
    {
        //Arrange
        await ServerUtil.RegisterUser(Page, "ATestUser", "test@mail.com", "Testpassword123!");
            
        //Act
        await Page.Locator("#Message").ClickAsync();
        await Page.Locator("#Message").FillAsync("This is a test cheep");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        //Assert
        await Expect(Page.GetByText("This is a test cheep")).ToBeVisibleAsync();
            
        //Act
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
            
        //Assert
        await Expect(Page.GetByText("This is a test cheep")).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task CheckingMyTimelineForOtherCheeps()
    {
        //Arrange
        await ServerUtil.RegisterUser(Page, "ATestUser", "test@mail.com", "Testpassword123!");
            
        //Act
        await Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the" }).Locator("#follow").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
            
        //Assert
        await Expect(Page.GetByText("Starbuck now is what we hear the worst.")).ToBeVisibleAsync();
            
        //Act
        await Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the" }).Locator("#unfollow").ClickAsync();
            
        //Assert
        await Expect(Page.GetByText("Starbuck now is what we hear the worst.")).Not.ToBeVisibleAsync();
    }
    
    [Test]
    public async Task UserFollowsAnotherUserAndUnfollows()
    {
        //Act
        await ServerUtil.RegisterUser(Page, "ATestUser", "test@mail.com", "Testpassword123!");

        var buttonLocator = Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the" })
            .Locator("#follow");
        string buttonText = await buttonLocator.InnerTextAsync();

        //Assert
        Assert.That(buttonText, Is.EqualTo("Follow"));

        //Act
        await Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the worst" })
            .Locator("#follow").ClickAsync();

        var buttonLocator2 = Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the" })
            .Locator("#unfollow");
        string buttonText2 = await buttonLocator2.InnerTextAsync();

        //Assert
        Assert.That(buttonText2, Is.EqualTo("Unfollow"));

        //Act
        await Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the worst" })
            .Locator("#unfollow").ClickAsync();

        var buttonLocator3 = Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the" })
            .Locator("#follow");
        string buttonText3 = await buttonLocator3.InnerTextAsync();

        //Assert
        Assert.That(buttonText3, Is.EqualTo("Follow"));
    }
}