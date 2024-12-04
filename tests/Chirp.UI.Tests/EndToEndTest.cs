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
            await ServerUtil.DeleteUser(Page, "test@mail.com", "Testpassword123!");
            await ServerUtil.DeleteUser(Page, "testmail@mail.com", "Testpassword123!");
            await ServerUtil.DeleteUser(Page, "atest@mail.com", "Testpassword123!");
        }

        [Test]
        public async Task UserRegistersANewAccountIsDirectlySignedInLogsOutAndLogsInWithNewAccountLogsOutLogsInDeletesAccount()
        {
            //Arrange
            await Page.GotoAsync("https://localhost:5273/");
         
            //Act
            await ServerUtil.RegisterUser(Page, "username1");
           
            //Act
            await Page.GetByRole(AriaRole.Button, new() { Name = "logout [username1]" }).ClickAsync();
            await ServerUtil.LoginUser(Page);

            //Assert
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "logout [username1]" })).ToBeVisibleAsync();

            //Act
            await Page.GetByRole(AriaRole.Button, new() { Name = "logout [username1]" }).ClickAsync();

            //Assert
            await Expect(Page.GetByText("login")).ToBeVisibleAsync();

            //Act
            await ServerUtil.LoginUser(Page);
            await ServerUtil.DeleteUser(Page);
            await ServerUtil.LoginUser(Page);

            //Assert
            await Expect(Page.GetByText("No user found")).ToBeVisibleAsync();
        }

        
        [Test]
        public async Task UserRegistersANewAccountAndLogsInWithNewAccountWritesCheepDeletesAccount()
        {
            //Arrange
            await Page.GotoAsync("https://localhost:5273/");
         
            //Act
            await ServerUtil.RegisterUser(Page, "username5");
           
            //Act
            await Page.GetByRole(AriaRole.Button, new() { Name = "logout [username5]" }).ClickAsync();
            await ServerUtil.LoginUser(Page);

            //Assert
            await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "logout [username5]" })).ToBeVisibleAsync();

            //Act
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("This is a test cheep");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

            //Assert
            await Expect(Page.GetByText("This is a test cheep")).ToBeVisibleAsync();

            //Act
            await ServerUtil.DeleteUser(Page);
            await ServerUtil.LoginUser(Page);

            //Assert
            await Expect(Page.GetByText("No user found")).ToBeVisibleAsync();
        }
        
        [Test]
        public async Task UserFollowsAccountsAndWritesCheepsCorrectAmountOfAccountsAndCheepsInAboutMePage()
        {
            //Arrange
            await ServerUtil.RegisterUser(Page, "ATestUser1", "atest@mail.com", "Testpassword123!");

            //Act
            await Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the worst" }).Locator("#follow").ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "But what was" }).Locator("#follow").ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "At present I" }).Locator("#follow").ClickAsync();
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
            var follower2 = Page.GetByRole(AriaRole.Link, new() { Name = "Malcolm Janski" });
            var follower3 = Page.GetByRole(AriaRole.Link, new() { Name = "Mellie Yost" });

            Assert.IsTrue(await follower1.IsVisibleAsync());
            Assert.IsTrue(await follower2.IsVisibleAsync());
            Assert.IsTrue(await follower3.IsVisibleAsync());

            //Act 
            await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "Hello World3" }).GetByRole(AriaRole.Button).ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "Hello World2" }).GetByRole(AriaRole.Button).ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "Hello World1" }).GetByRole(AriaRole.Button).ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the worst." }).Locator("#unfollow").ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "But what was behind the barricade." }).Locator("#unfollow").ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "At present I cannot spare energy and determination such as I did look up I saw a gigantic Sperm Whale is toothless." }).Locator("#unfollow").ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();

            //Assert
            await Expect(Page.GetByText("Hello World1")).ToBeHiddenAsync();
            await Expect(Page.GetByText("Hello World2")).ToBeHiddenAsync();
            await Expect(Page.GetByText("Hello World3")).ToBeHiddenAsync();

            Assert.IsFalse(await follower1.IsVisibleAsync());
            Assert.IsFalse(await follower2.IsVisibleAsync());
            Assert.IsFalse(await follower3.IsVisibleAsync());
        }

        [Test]
        public async Task TestUserFollowsAccount()
        {
            //Arrange
            await ServerUtil.RegisterUser(Page, "ATestUser", "hello@mail.com", "Testpassword123!");
            await Page.Locator("#Message").ClickAsync();
            await Page.Locator("#Message").FillAsync("Hello world");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "logout [Atestuser]" }).ClickAsync();

            //Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
            await Page.GetByPlaceholder("user name").ClickAsync();
            await Page.GetByPlaceholder("user name").FillAsync("Atestuser2");
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("hello2@mail.com");
            await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
            await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Password!123");
            await Page.GetByLabel("Confirm Password").ClickAsync();
            await Page.GetByLabel("Confirm Password").FillAsync("Password!123");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "Atestuser — 12/04/24 13:10:32" }).GetByRole(AriaRole.Button).Nth(1).ClickAsync();
            await Page.Locator("li").Filter(new() { HasText = "Atestuser — 12/04/24 13:10:32" }).Locator("#follow").ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "logout [Atestuser2]" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("hello@mail.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Password!123");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "about me" }).ClickAsync();
            await Page.GetByRole(AriaRole.Button, new() { Name = "logout [Atestuser]" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("hello2@mail.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Password!123");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "manage account" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Personal data" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").FillAsync("Password!123");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "login" }).ClickAsync();
            await Page.GetByPlaceholder("name@example.com").ClickAsync();
            await Page.GetByPlaceholder("name@example.com").FillAsync("hello@mail.com");
            await Page.GetByPlaceholder("password").ClickAsync();
            await Page.GetByPlaceholder("password").FillAsync("Password!123");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "about me" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "manage account" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Personal data" }).ClickAsync();
            await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").FillAsync("Password!123");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();

            //Assert
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
            await Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the worst" }).Locator("#follow").ClickAsync();
            
            var buttonLocator2 = Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the" })
                .Locator("#unfollow");
            string buttonText2 = await buttonLocator2.InnerTextAsync();
            
            //Assert
            Assert.That(buttonText2, Is.EqualTo("Unfollow"));
            
            //Act
            await Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the worst" }).Locator("#unfollow").ClickAsync();
            
            var buttonLocator3 = Page.Locator("li").Filter(new() { HasText = "Starbuck now is what we hear the" })
                .Locator("#follow");
            string buttonText3 = await buttonLocator3.InnerTextAsync();
            
            //Assert
            Assert.That(buttonText3, Is.EqualTo("Follow"));
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
    }
}