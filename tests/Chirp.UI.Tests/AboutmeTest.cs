using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Xunit;

namespace Chirp.UI.Tests;


public class AboutmeTest : PageTest
{
    private Process _serverProcess;
    
    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _serverProcess = await ServerUtil.StartServer();
    }
        
    [SetUp]
    public async Task Setup()
    {
        await RegisterUser();
        await LoginUser();
    }
    
    [TearDown]
    public async Task TearDown()
    {
        await LoginUser();
        await DeleteUser();
    }
        
    [OneTimeTearDown]
    public void OneTimeCleanup()
    {
        if (_serverProcess != null && !_serverProcess.HasExited)
        {
            _serverProcess.Kill();
            _serverProcess.Dispose();
        }
    }
    
    [Test]
    public async Task CheepBoxIsNotVisibleWhenNotLoggedIn()
    {
        // Arrange
        await Page.GotoAsync("https://localhost:5273/Identity/Account/AboutMe/InfoPage");
        
        // Act
        
        //var listItemText = await Page.Locator("li").TextContentAsync();
        
        var listItemText = await Page.Locator("ul li").TextContentAsync();

        // Extract the username using a regular expression
        var match = Regex.Match(listItemText, @"Your Username is: (.+)");
            
        // Assert
        //Assert.Equals("user name", match);
        //Assert.Equals(listItemText., match);
    }
    
    // This method can be used to prepare a test that requires a logged in user
    public async Task LoginUser(string email = "name@example.com", string password = "Password123!")
    {

        var isUserLoggedIn = await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).IsVisibleAsync();
        if (isUserLoggedIn)
        {
            return;
        }
        
        //Arrange
        await Page.GotoAsync("https://localhost:5273/Identity/Account/Login");

        await Page.GetByPlaceholder("name@example.com").FillAsync(email);
        await Page.GetByPlaceholder("password").FillAsync(password);
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        var myTimelineButton = Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" });
        await Expect(myTimelineButton).ToBeVisibleAsync();

    }
    
    public async Task RegisterUser()
    {
        await Page.GotoAsync("https://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
        await Page.GetByPlaceholder("user name").ClickAsync();
        
        await Page.GetByPlaceholder("user name").FillAsync("username");
        await Page.GetByPlaceholder("name@example.com").FillAsync("name@example.com");
        await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password123!");
        await Page.GetByLabel("Confirm Password").FillAsync("Password123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        
        //await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
    }

    public async Task DeleteUser()
    {
        var isUserLoggedIn = await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).IsVisibleAsync();
        if (!isUserLoggedIn)
        {
            await LoginUser();
        }
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "manage account" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Personal data" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Password123!");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
    }
}