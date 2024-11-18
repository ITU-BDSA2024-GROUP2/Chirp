using NUnit.Framework;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            await LoginUserAndDeleteUser("test@mail.com", "Testpassword123!");
            await LoginUserAndDeleteUser("testmail@mail.com", "Testpassword123!");
        }

        [Test]
        public async Task UserRegistersANewAccountAndLogsInWithNewAccountLogsOutLogsInDeletesAccount()
        {   
            //Arrange
            await Page.GotoAsync("https://localhost:5273/");
            
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
            //await Expect(Page.GetByText("Register confirmation")).ToBeVisibleAsync();

            //Act
            //await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("test@mail.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Testpassword123!");
            await Page.GetByLabel("Remember me?").CheckAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            
            //Assert
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "logout [TestUser]" })).ToBeVisibleAsync();

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
            await Page.GetByPlaceholder("user name").FillAsync("ATestUser");
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("testmail@mail.com");
            await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
            await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Testpassword123!");
            await Page.GetByLabel("Confirm Password").ClickAsync();
            await Page.GetByLabel("Confirm Password").FillAsync("Testpassword123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

            //Assert
            //await Expect(Page.GetByText("Register confirmation")).ToBeVisibleAsync();

            //Act
            //await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("testmail@mail.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Testpassword123!");
            await Page.GetByLabel("Remember me?").CheckAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            
            //Assert
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "logout [ATestUser]" })).ToBeVisibleAsync();

            //Act
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("This is a test cheep");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

            //Assert
            await Expect(Page.GetByText("This is a test cheep")).ToBeVisibleAsync();

            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "manage account" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Personal data" }).ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").FillAsync("Testpassword123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("testmail@mail.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Testpassword123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

            //Assert
            await Expect(Page.GetByText("No user found")).ToBeVisibleAsync();
        }
        
        public async Task LoginUserAndDeleteUser(string email, string password)
        {
            await Page.GotoAsync("https://localhost:5273/Identity/Account/Login");

            await Page.GetByPlaceholder("name@example.com").FillAsync(email);
            await Page.GetByPlaceholder("password").FillAsync(password);
            
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            
            var isUserLoggedIn = await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).IsVisibleAsync();
            if (!isUserLoggedIn)
            {
                return;
            }
            await Page.GetByRole(AriaRole.Link, new() { Name = "manage account" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Personal data" }).ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").FillAsync("Testpassword123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
        }
    }
}