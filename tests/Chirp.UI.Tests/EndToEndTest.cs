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
            await ServerUtil.LoginUserAndDeleteUser("test@mail.com", "Testpassword123!", Page);
            await ServerUtil.LoginUserAndDeleteUser("testmail@mail.com", "Testpassword123!", Page);
        }

        [Test]
        public async Task
            UserRegistersANewAccountIsDirectlySignedInLogsOutAndLogsInWithNewAccountLogsOutLogsInDeletesAccount()
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
            
            //Act
            await Page.GetByText("logout").ClickAsync();
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
            await Page.GetByRole(AriaRole.Button, new() { Name = "logout [ATestUser]" }).ClickAsync();

            //Assert
            await Expect(Page.GetByText("login")).ToBeVisibleAsync();

            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("testmail@mail.com");
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
            await Page.GetByPlaceholder("name@example.com").FillAsync("testmail@mail.com");
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
            await Page.GetByText("logout").ClickAsync();

            //Act
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

[Test]
        public async Task UserFollowsAccountsAndWritesCheepsCorrectAmountOfAccountsAndCheepsInAboutMePage()
        {
            //Arrange
            await Page.GotoAsync("https://localhost:5273/");

            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
            await Page.GetByPlaceholder("user name").ClickAsync();
            await Page.GetByPlaceholder("user name").FillAsync("ATestUser1");
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("atest@mail.com");
            await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
            await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Testpassword123!");
            await Page.GetByLabel("Confirm Password").ClickAsync();
            await Page.GetByLabel("Confirm Password").FillAsync("Testpassword123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).Locator("#follow").ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "Mellie Yost But what was" }).Locator("#follow").ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "Malcolm Janski At present I" }).Locator("#follow").ClickAsync();
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("Hello World1");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("Hello World2");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("Hello World3");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();

            //Assert
            await Expect(Page.GetByText("Hello World1")).ToBeVisibleAsync();
            await Expect(Page.GetByText("Hello World2")).ToBeVisibleAsync();
            await Expect(Page.GetByText("Hello World3")).ToBeVisibleAsync();

            var follower1 = Page.GetByRole(AriaRole.Link, new() { Name = "Jacqualine Gilcoine" });
            var follower2 = Page.GetByRole(AriaRole.Link, new() { Name = "Quintin Sitts" });
            var follower3 = Page.GetByRole(AriaRole.Link, new() { Name = "Mellie Yost" });

            Assert.IsTrue(await follower1.IsVisibleAsync());
            Assert.IsTrue(await follower2.IsVisibleAsync());
            Assert.IsTrue(await follower3.IsVisibleAsync());

            //Act 
            await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "ATestUser1 Hello World3 — 11/" }).GetByRole(AriaRole.Button).ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "ATestUser1 Hello World2 — 11/" }).GetByRole(AriaRole.Button).ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "ATestUser1 Hello World — 11/" }).GetByRole(AriaRole.Button).ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).Locator("#unfollow").ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "Mellie Yost But what was" }).Locator("#unfollow").ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "Malcolm Janski At present I" }).Locator("#unfollow").ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();

            //Assert

        }
        
        [Test]
        public async Task UserFollowsAnotherUserAndUnfollows()
        {   
            //Act
            await ServerUtil.RegisterUser("ATestUser", "test@mail.com", "Testpassword123!", Page);
            
            
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
        }
        
        [Test]
        public async Task CheckingMyTimelineForOtherCheeps()
        {
            //Arrange
            await ServerUtil.RegisterUser("ATestUser", "test@mail.com", "Testpassword123!", Page);
            
            //Act
            await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Button).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
            
            //Assert
            await Expect(Page.GetByText("Jacqualine Gilcoine Starbuck")).ToBeVisibleAsync();
            
            //Act
            await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Button).ClickAsync();
            
            //Assert
            await Expect(Page.GetByText("Jacqualine Gilcoine Starbuck")).Not.ToBeVisibleAsync();
            
        }
        
        [Test]
        public async Task UserChecksMyTimelineForOwnCheeps()
        {
            //Arrange
            await ServerUtil.RegisterUser("ATestUser", "test@mail.com", "Testpassword123!", Page);
            
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
        public async Task UserIsAbleToMakeAndDeleteCheeps()
        {
            //Arrange
            await ServerUtil.RegisterUser("ATestUser", "test@mail.com", "Testpassword123!", Page);
            
            //Act
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("This is a test cheep");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            
            var buttonLocator = Page.Locator("li").Filter(new() { HasText = "ATestUser This is a test cheep" }).GetByRole(AriaRole.Button);
            string buttonText = await buttonLocator.InnerTextAsync();
            
            //Assert
            Assert.That(buttonText, Is.EqualTo("Delete"));
            
            //Act
            await Page.Locator("li").Filter(new() { HasText = "ATestUser This is a test cheep" }).GetByRole(AriaRole.Button).ClickAsync();
            
            //Assert
            await Expect(Page.GetByText("ATestUser This is a test cheep")).Not.ToBeVisibleAsync();
            
            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
            
            //Assert
            await Expect(Page.GetByText("ATestUser This is a test cheep")).Not.ToBeVisibleAsync();
            
            //Act
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("This is a test cheep on my timeline");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            
            //Assert
            await Expect(Page.GetByText("This is a test cheep on my timeline")).ToBeVisibleAsync();
            
            //Act
            await Page.Locator("li").Filter(new() { HasText = "ATestUser This is a test cheep on my timeline"}).GetByRole(AriaRole.Button).ClickAsync();
            
            //Assert
            await Expect(Page.GetByText("This is a test cheep on my timeline")).Not.ToBeVisibleAsync();
        }
    }
}