using NUnit.Framework;
using System.Diagnostics;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Chirp.UI.Tests
{
    public class Chirp_UI_SendCheep_Tests : PageTest
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
        public async Task OneTimeCleanup()
        {
            if (_serverProcess != null && !_serverProcess.HasExited)
            {
                _serverProcess.Kill();
                _serverProcess.Dispose();
            }
        }
        
        [Test]
        public async Task CheepBoxIsVisibleWhenLoggedIn()
        {
            // Arrange
            await Page.GotoAsync("http://localhost:5273");
            
            // Act
            var shareButton = Page.GetByRole(AriaRole.Button, new() { Name = "Share" });
            
            // Assert
            await Expect(shareButton).ToBeVisibleAsync();
        }
        
        [Test]
        public async Task CheepBoxIsNotVisibleWhenNotLoggedIn()
        {
            // Arrange
            var isUserLoggedIn = await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).IsVisibleAsync();
            if (isUserLoggedIn)
            {
                await Page.GetByRole(AriaRole.Button, new() { Name = "logout [username]" }).ClickAsync();
            }
            
            // Act
            await Page.GotoAsync("http://localhost:5273");
            var shareButton = Page.GetByRole(AriaRole.Button, new() { Name = "Share" });
            
            // Assert
            await Expect(shareButton).ToBeHiddenAsync();
        }
        
        [Test]
        public async Task SendingCheepShowsNewCheepOnPublicTimeline()
        {
            // Arrange
            await Page.GotoAsync("http://localhost:5273");
            
            int randCheepId = new Random().Next();
            
            // Act
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("Cheeping cheeps on Chirp!" + randCheepId);
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            
            var newCheep = Page.Locator("li").Filter(new() { HasText = "username Cheeping cheeps on Chirp!" + randCheepId }).First;
            
            // Assert
            await Expect(newCheep).ToBeVisibleAsync();
            await Expect(newCheep).ToContainTextAsync("Cheeping cheeps on Chirp!" + randCheepId);
        }

        [Test]
        public async Task CheepboxDoesNotAllowUserToSendCheepLongerThan160Characters()
        {
            // Arrange
            await Page.GotoAsync("http://localhost:5273");
            
            int randCheepId = new Random().Next();
            string longCheepMessage = new string('a', 160) + randCheepId;
            
            // Act
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync(longCheepMessage);
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            
            await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
            
            var newCheep =  Page.Locator("li").Filter(new() { HasText = longCheepMessage.Substring(0, 160) }).First;

            // Assert
            await Expect(newCheep).ToBeVisibleAsync();
            await Expect(newCheep).ToContainTextAsync(longCheepMessage.Substring(0, 160));
            string cheepText = await newCheep.InnerTextAsync();
            Assert.That(cheepText, Does.Not.Contain(randCheepId.ToString()));

        }
        
        [Test]
        public async Task CheepboxDoesNotAllowUserToSendEmptyCheeps()
        {
            // Arrange
            await Page.GotoAsync("http://localhost:5273");
            
            // Act
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            var cheepbox = Page.GetByText("Cheep cannot be empty Share");

            // Assert
            await Expect(cheepbox).ToBeVisibleAsync();
        }
        
        [Test]
        public async Task SendingCheepShowCheepOnPrivateTimelineForRespectiveAuthor()
        {
            // Arrange
            await Page.GotoAsync("http://localhost:5273");
            
            int randCheepId = new Random().Next();
            
            // Act
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("Cheeping cheeps on Chirp!" + randCheepId);
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            
            await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
            var newCheep = Page.Locator("li").Filter(new() { HasText = "username Cheeping cheeps on Chirp!" + randCheepId }).First;
            
            // Assert
            await Expect(newCheep).ToBeVisibleAsync();
            await Expect(newCheep).ToContainTextAsync("Cheeping cheeps on Chirp!" + randCheepId);
        }

        [Test]
        public async Task CheepboxIsNotVulnerableForXSSAttacks()
        {
            // Arrange
            await Page.GotoAsync("http://localhost:5273");
            
            // Act
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("Hello, I am feeling good!<script>alert('If you see this in a popup, you are in trouble!');</script>");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            
            var newCheep = Page.Locator("li").Filter(new() { HasText = "<script>alert('If you see this in a popup, you are in trouble!');</script>"}).First;
            
            // Assert
            await Expect(newCheep).ToBeVisibleAsync();
            await Expect(newCheep)
                .ToContainTextAsync("<script>alert('If you see this in a popup, you are in trouble!');</script>");
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
            await Page.GotoAsync("http://localhost:5273/Identity/Account/Login");

            await Page.GetByPlaceholder("name@example.com").FillAsync(email);
            await Page.GetByPlaceholder("password").FillAsync(password);
            
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

            var myTimelineButton = Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" });
            await Expect(myTimelineButton).ToBeVisibleAsync();

        }
        
        public async Task RegisterUser()
        {
            await Page.GotoAsync("http://localhost:5273/");
            await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
            await Page.GetByPlaceholder("user name").ClickAsync();
            
            await Page.GetByPlaceholder("user name").FillAsync("username");
            await Page.GetByPlaceholder("name@example.com").FillAsync("name@example.com");
            await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password123!");
            await Page.GetByLabel("Confirm Password").FillAsync("Password123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
            
            await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
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
}