using NUnit.Framework;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Chirp.UI.Tests
{
    public class EndToEndTest : PageTest
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
            await ServerUtil.LoginUser(Page);
            await ServerUtil.DeleteUser(Page);
        }

        [Test]
        public async Task UserRegistersANewAccountIsDirectlySignedInLogsOutAndLogsInWithNewAccountLogsOutLogsInDeletesAccount()
        {
            //Arrange
            await Page.GotoAsync("https://localhost:5273/");

            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
            await Page.GetByPlaceholder("user name").ClickAsync();
            await Page.GetByPlaceholder("user name").FillAsync("username");
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("name@example.com");
            await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
            await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password123!");
            await Page.GetByLabel("Confirm Password").ClickAsync();
            await Page.GetByLabel("Confirm Password").FillAsync("Password123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
            
            //Act
            await Page.GetByText("logout").ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("name@example.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Password123!");
            await Page.GetByLabel("Remember me?").CheckAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

            //Assert
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "logout [username]" })).ToBeVisibleAsync();

            //Act
            await Page.GetByRole(AriaRole.Button, new() { Name = "logout [username]" }).ClickAsync();

            //Assert
            await Expect(Page.GetByText("login")).ToBeVisibleAsync();

            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("name@example.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Password123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "manage account" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Personal data" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").FillAsync("Password123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("name@example.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Password123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

            //Assert
            await Expect(Page.GetByText("No user found")).ToBeVisibleAsync();
        }

        
        [Test]
        public async Task UserRegistersANewAccountAndLogsInWithNewAccountWritesCheepDeletesAccount()
        {
            //Arrange
            await Page.GotoAsync("https://localhost:5273/");

            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
            await Page.GetByPlaceholder("user name").ClickAsync();
            await Page.GetByPlaceholder("user name").FillAsync("username");
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("testmail@mail.com");
            await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
            await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password123!");
            await Page.GetByLabel("Confirm Password").ClickAsync();
            await Page.GetByLabel("Confirm Password").FillAsync("Password123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
            await Page.GetByText("logout").ClickAsync();

            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("name@example.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Password123!");
            await Page.GetByLabel("Remember me?").CheckAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

            //Assert
            await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "logout [username]" })).ToBeVisibleAsync();

            //Act
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("This is a test cheep");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

            //Assert
            await Expect(Page.GetByText("This is a test cheep")).ToBeVisibleAsync();

            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "manage account" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Personal data" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").FillAsync("Password123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("name@example.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Password123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

            //Assert
            await Expect(Page.GetByText("No user found")).ToBeVisibleAsync();
        }
        
        [Test]
        public async Task UserFollowsAnotherUserAndUnfollows()
        {   
            //Act
            await ServerUtil.RegisterUser(Page);
            
            
            var buttonLocator = Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Button);
            string buttonText = await buttonLocator.InnerTextAsync();
            
            //Assert
            Assert.That(buttonText, Is.EqualTo("Follow"));
            
            //Act
            await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Button).ClickAsync();
            var buttonLocator2 = Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Button);
            string buttonText2 = await buttonLocator2.InnerTextAsync();
            
            //Assert
            Assert.That(buttonText2, Is.EqualTo("Unfollow"));
            
            await ServerUtil.DeleteUser(Page);
        }
        
        [Test]
        public async Task CheckingMyTimelineForOtherCheeps()
        {
            //Arrange
            await ServerUtil.DeleteUser(Page);
            await ServerUtil.RegisterUser(Page);
            
            //Act
            await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Button).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
            
            //Assert
            await Expect(Page.GetByText("Jacqualine Gilcoine Starbuck")).ToBeVisibleAsync();
            
            //Act
            await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Button).ClickAsync();
            
            //Assert
            await Expect(Page.GetByText("Jacqualine Gilcoine Starbuck")).Not.ToBeVisibleAsync();
            
            await ServerUtil.DeleteUser(Page);
        }
        
        [Test]
        public async Task UserChecksMyTimelineForOwnCheeps()
        {
            //Arrange
            await ServerUtil.RegisterUser(Page);
            
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
            
            await ServerUtil.DeleteUser(Page);
        }

        [Test]
        public async Task UserIsAbleToMakeAndDeleteCheeps()
        {
            //Arrange
            await ServerUtil.RegisterUser(Page);
            
            //Act
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("This is a test cheep");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            
            var buttonLocator = Page.Locator("li").Filter(new() { HasText = "username This is a test cheep" }).GetByRole(AriaRole.Button);
            string buttonText = await buttonLocator.InnerTextAsync();
            
            //Assert
            Assert.That(buttonText, Is.EqualTo("DELETE"));
            
            //Act
            await Page.Locator("li").Filter(new() { HasText = "username This is a test cheep" }).GetByRole(AriaRole.Button).ClickAsync();
            
            //Assert
            await Expect(Page.GetByText("username This is a test cheep")).Not.ToBeVisibleAsync();
            
            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
            
            //Assert
            await Expect(Page.GetByText("username This is a test cheep")).Not.ToBeVisibleAsync();
            
            //Act
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("This is a test cheep on my timeline");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            
            //Assert
            await Expect(Page.GetByText("This is a test cheep on my timeline")).ToBeVisibleAsync();
            
            //Act
            await Page.Locator("li").Filter(new() { HasText = "username This is a test cheep on my timeline"}).GetByRole(AriaRole.Button).ClickAsync();
            
            //Assert
            await Expect(Page.GetByText("This is a test cheep on my timeline")).Not.ToBeVisibleAsync();
            
            await ServerUtil.DeleteUser(Page);
        }
    }
}