using NUnit.Framework;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Chirp.UI.Tests
{
    public class EndToEndTest : PageTest
    {
        private Process _serverProcess;
        
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

        [Test]
        public async Task UserResgistersANewAccountAndLogsInWithNewAccountLogsOutLogsInDeletesAccount()
        {   
            //Arrange
            await Page.GotoAsync("http://localhost:5273/");
            
            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
            await Page.GetByPlaceholder("user name").ClickAsync();
            await Page.GetByPlaceholder("user name").FillAsync("TestUser");
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
            await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
            await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Testpassword123!");
            await Page.GetByLabel("Confirm Password").ClickAsync();
            await Page.GetByLabel("Confirm Password").FillAsync("Testpassword123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

            //Assert
            await Expect(Page.GetByText("Register confirmation")).ToBeVisibleAsync();

            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Testpassword123!");
            await Page.GetByLabel("Remember me?").CheckAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            
            //Assert
            await Expect(Page.GetByText("TestUser")).ToBeVisibleAsync();

            //Act
            await Page.GetByRole(AriaRole.Button, new() { Name = "logout [TestUser]" }).ClickAsync();

            //Assert
            await Expect(Page.GetByText("login")).ToBeVisibleAsync();

            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Testpassword123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "manage account" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Personal data" }).ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").FillAsync("Testpassword123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Testpassword123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

            //
            //await Expect(Page.GetByText("invalid login")).ToBeVisibleAsync();
        }
        
        [Test]
        public async Task CheepBoxIsVisibleWhenLoggedIn()
        {
            await LoginUser();
            
            await Page.GotoAsync("http://localhost:5273");


            var shareButton = Page.GetByRole(AriaRole.Button, new() { Name = "Share" });
            await Expect(shareButton).ToBeVisibleAsync();
        }
        
        [Test]
        public async Task CheepBoxIsNotVisibleWhenNotLoggedIn()
        {
            var isUserLoggedIn = await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).IsVisibleAsync();
            if (isUserLoggedIn)
            {
                await Page.GetByRole(AriaRole.Button, new() { Name = "logout [username]" }).ClickAsync();
            }
            
            await Page.GotoAsync("http://localhost:5273");


            var shareButton = Page.GetByRole(AriaRole.Button, new() { Name = "Share" });
            await Expect(shareButton).ToBeHiddenAsync();
        }
        
        [Test]
        public async Task SendingCheepShowsNewCheepOnTimeline()
        {
            await LoginUser();
            
            int randCheepId = new Random().Next();
            
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("Cheeping cheeps on Chirp!" + randCheepId);
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            
            var newCheep = Page.Locator("li").Filter(new() { HasText = "username Cheeping cheeps on Chirp!" + randCheepId }).First;
            await Expect(newCheep).ToBeVisibleAsync();
            await Expect(newCheep).ToContainTextAsync("Cheeping cheeps on Chirp!" + randCheepId);
        }
        
        [Test]
        public async Task SendingCheepStoresCheepInDatabase()
        {
            
        }
        
        [Test]
        public async Task CheepboxDoesNotAllowUserToSendCheepLongerThan160Characters()
        {
        
        }
        
        [Test]
        public async Task SendingCheepStoresCheepForRespectiveAuthor()
        {
        
        }
        
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
    }
}