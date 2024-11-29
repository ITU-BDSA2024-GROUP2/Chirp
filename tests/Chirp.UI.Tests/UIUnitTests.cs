using NUnit.Framework;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Chirp.UI.Tests
{
    public class UIUnitTests : PageTest
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
        
        [Test]
        public async Task TestAddress()
        {   
            //Arrange
            bool httpsUsed = false;
            bool httpUsed = false; 
            
            //Act
            try
            {
                await Page.GotoAsync("https://localhost:5273");
                httpsUsed = true;
            }
            catch (Exception e)
            {
                string dummy = e.Message;
            }

            try
            {
                await Page.GotoAsync("http://localhost:5273");
                httpsUsed = true;
            }
            catch (Exception e)
            {
                string dummy = e.Message;
            }
            
            //Assert
            Assert.IsTrue(httpsUsed);
            Assert.IsFalse(httpUsed);
        }
        
        [Test]
        public async Task TestTitle()
        {   
            //Arrange
            
            //Act
            await Page.GotoAsync("https://localhost:5273");
            
            //Assert
            await Expect(Page).ToHaveTitleAsync(new Regex("Chirp"));
            
        }
        
        [Test]
        public async Task TestClickOnUser()
        {
            //Arrange
            await Page.GotoAsync("https://localhost:5273");
            
            //Act
            var locator = Page.Locator("li").Filter(new()
                    { HasText = "Jacqualine Gilcoine — 08/01/23 13:17:39 Starbuck now is what we hear the worst." })
                .GetByRole(AriaRole.Link);
            
            //Assert
            await Expect(locator).ToHaveAttributeAsync("href", "/Jacqualine Gilcoine");
            
        }
        
        [Test]
        public async Task TestClickLoginAndRegister()
        {
            //Arrange
            await Page.GotoAsync("https://localhost:5273");
            
            //Act
            var locator = Page.GetByRole(AriaRole.Link, new() { Name = "login" });
            var locator2 = Page.GetByRole(AriaRole.Link, new() { Name = "register" });
            
            //Assert
            await Expect(locator).ToHaveAttributeAsync("href", "/Identity/Account/Login");
            await Expect(locator2).ToHaveAttributeAsync("href", "/Identity/Account/Register");
            
        }
        
        [Test]
        public async Task TestClickOnLogo()
        {
            //Arrange
            await Page.GotoAsync("https://localhost:5273/Identity/Account/Login");
            
            //Act
            var locator = Page.GetByRole(AriaRole.Link, new() { Name = "Icon1" });
            
            //Assert
            await Expect(locator).ToHaveAttributeAsync("href", "/");
            
        }
        
        [Test]
        public async Task TestLoginForm()
        {
            //Arrange
            await Page.GotoAsync("https://localhost:5273/Identity/Account/Login");
            
            //Act
            var email = Page.Locator("input[name='Input.Email']");
            var password = Page.Locator("input[name='Input.Password']");
            
            //Assert
            await Expect(email).ToBeVisibleAsync();
            await Expect(password).ToBeVisibleAsync();
            
        }
        
        [Test]
        public async Task TestRegisterForm()
        {
            //Arrange
            await Page.GotoAsync("https://localhost:5273/Identity/Account/Register");
            
            //Act
            var username = Page.Locator("input[name='Input.UserName']");
            var email = Page.Locator("input[name='Input.Email']");
            var password = Page.Locator("input[name='Input.Password']");
            var confirmPassword = Page.Locator("input[name='Input.ConfirmPassword']");
            
            //Assert
            await Expect(username).ToBeVisibleAsync();
            await Expect(email).ToBeVisibleAsync();
            await Expect(password).ToBeVisibleAsync();
            await Expect(confirmPassword).ToBeVisibleAsync();

        }
        
        [Test]
        public async Task TestInvalidValidUserName()
        {
            // Arrange
            await Page.GotoAsync("https://localhost:5273/Identity/Account/Register");

            // Act
            var nameInput = Page.Locator("input[name='Input.UserName']");
            await nameInput.FillAsync("");
           
            var registerButton = Page.Locator("#registerSubmit");
            await registerButton.ClickAsync();

            // Assert
            var usernameValidationMessage = Page.Locator("span[data-valmsg-for='Input.UserName']");

            // Verify that the validation message is visible and contains the expected text
            await Expect(usernameValidationMessage).ToBeVisibleAsync();
            await Expect(usernameValidationMessage).ToHaveTextAsync("The User name field is required.");
        }
        
        [Test]
        public async Task TestInvalidValidEmailNoEntry()
        {
            // Arrange
            await Page.GotoAsync("https://localhost:5273/Identity/Account/Register");

            // Act
            var emailInput = Page.Locator("input[name='Input.Email']");
            await emailInput.FillAsync("");
           
            var registerButton = Page.Locator("#registerSubmit");
            await registerButton.ClickAsync();

            // Assert
            var emailValidationMessage = Page.Locator("span[data-valmsg-for='Input.Email']");

            // Verify that the validation message is visible and contains the expected text
            await Expect(emailValidationMessage).ToBeVisibleAsync();
            await Expect(emailValidationMessage).ToHaveTextAsync("The Email field is required.");
        }
        
        [Test]
        public async Task TestInvalidValidPasswordNoEntry()
        {
            // Arrange
            await Page.GotoAsync("https://localhost:5273/Identity/Account/Register");

            // Act
            var passwordInput = Page.Locator("input[name='Input.Password']");
            await passwordInput.FillAsync("");
           
            var registerButton = Page.Locator("#registerSubmit");
            await registerButton.ClickAsync();

            // Assert
            var passwordValidationMessage = Page.Locator("span[data-valmsg-for='Input.Password']");

            // Verify that the validation message is visible and contains the expected text
            await Expect(passwordValidationMessage).ToBeVisibleAsync();
            await Expect(passwordValidationMessage).ToHaveTextAsync("The Password field is required.");
        }

        /*[Test] // can't confirm this actually works,
         because the error popup can't be captured by playwright directly
        public async Task TestValidEmail()
        {
            
            await Page.GotoAsync("http://localhost:5273/Identity/Account/Register");

            
            var emailInput = Page.Locator("input[name='Input.Email']");

            
            await emailInput.FillAsync("valid.email@example.com");

           
            var registerButton = Page.Locator("#registerSubmit");
            await registerButton.ClickAsync();

            var validationMessage = Page.Locator("#Input_Email + span"); 
            await Expect(validationMessage).Not.ToBeVisibleAsync(); 
        }
        
        [Test]
        public async Task TestInvalidEmail()
        {
            
            await Page.GotoAsync("http://localhost:5273/Identity/Account/Register");

            
            var emailInput = Page.Locator("input[name='Input.Email']");

            
            await emailInput.FillAsync("invalidEmail.com");

           
            var registerButton = Page.Locator("#registerSubmit");
            await registerButton.ClickAsync();

            var validationMessage = Page.Locator("span[data-valmsg-for='Input.Email']");
            Console.WriteLine("the validation message: " + validationMessage);
            await Expect(validationMessage).ToBeVisibleAsync(); 
        }*/
        
        /*[Test]	
        public async Task TestValidPassword()
        {
            await Page.GotoAsync("http://localhost:5273/Identity/Account/Register");
            
            var passwordInput = Page.Locator("input[name='Input.Password']");
            
            await passwordInput.FillAsync("validPassword@example.com");
            
        }*/
        
        [Test]	
        public async Task TestInvalidValidPasswordLength()
        {
            // Arrange
            await Page.GotoAsync("https://localhost:5273/Identity/Account/Register");
            
            var emailInput = Page.Locator("input[name='Input.Email']");
            await emailInput.FillAsync("valid.email@example.com");
            
            // Act
            var passwordInput = Page.Locator("input[name='Input.Password']");
            await passwordInput.FillAsync("iamc");
            var registerButton = Page.Locator("#registerSubmit");
            await registerButton.ClickAsync();
            
            // Assert
            var passwordValidationMessage = Page.Locator("span[data-valmsg-for='Input.Password']");

            // Verify that the validation message is visible and contains the expected text
            await Expect(passwordValidationMessage).ToBeVisibleAsync();
            await Expect(passwordValidationMessage).ToHaveTextAsync("The Password must be at least 6 and at max 100 characters long.");
        }
        
        [Test]	
        public async Task TestInvalidValidPasswordListOfCriteria()
        {
            // Arrange
            await Page.GotoAsync("https://localhost:5273/Identity/Account/Register");
            
            var nameInput = Page.Locator("input[name='Input.UserName']");
            await nameInput.FillAsync("Jens");
            
            var emailInput = Page.Locator("input[name='Input.Email']");
            await emailInput.FillAsync("valid.email@example.com");
            
            // Act
            var passwordInput = Page.Locator("input[name='Input.Password']");
            await passwordInput.FillAsync("bobthebuilder");
            var confirmPasswordInput = Page.Locator("input[name='Input.ConfirmPassword']");
            await confirmPasswordInput.FillAsync("bobthebuilder");
            
            var registerButton = Page.Locator("#registerSubmit");
            await registerButton.ClickAsync();
            
            // Assert
            var passwordErrorItems = Page.Locator("div.validation-summary-errors ul li");

            // Verify that each validation message is visible and check for specific text
            await Expect(passwordErrorItems.Nth(0)).ToHaveTextAsync("Passwords must have at least one non alphanumeric character.");
            await Expect(passwordErrorItems.Nth(1)).ToHaveTextAsync("Passwords must have at least one digit ('0'-'9').");
            await Expect(passwordErrorItems.Nth(2)).ToHaveTextAsync("Passwords must have at least one uppercase ('A'-'Z').");
        }
        
        [Test]	
        public async Task TestConfirmPasswordNotSameAsPassword()
        {
            // Arrange
            await Page.GotoAsync("https://localhost:5273/Identity/Account/Register");
            
            var emailInput = Page.Locator("input[name='Input.Email']");
            await emailInput.FillAsync("valid.email@example.com");
            
            // Act
            var passwordInput = Page.Locator("input[name='Input.Password']");
            await passwordInput.FillAsync("Valid@Password12");
            var confirmPasswordInput = Page.Locator("input[name='Input.ConfirmPassword']");
            await confirmPasswordInput.FillAsync("Valid@Password11");
            
            var registerButton = Page.Locator("#registerSubmit");
            await registerButton.ClickAsync();
            
            // Assert
            var confirmPasswordValidationMessage = Page.Locator("span[data-valmsg-for='Input.ConfirmPassword']");

            // Verify that the validation message is visible and contains the expected text
            await Expect(confirmPasswordValidationMessage).ToBeVisibleAsync();
            await Expect(confirmPasswordValidationMessage).ToHaveTextAsync("The password and confirmation password do not match.");
        }
        
        /*[Test]	
        public async Task TestGithubLoginRedirect()
        {
            // Arrange
            await Page.GotoAsync("http://localhost:5273/Identity/Account/Register");
            
            // Act
            var githubButton = Page.Locator("button.provider-button.github-button");
            await githubButton.ClickAsync();
            
            
            string currentUrl = Page.Url;
            Console.WriteLine("Current URL after redirection: " + currentUrl);

            if (currentUrl.Contains("https://github.com/login"))
            {
                Console.WriteLine("Redirection to GitHub login page was successful!");
            }
            else
            {
                Console.WriteLine("Redirection failed or went to an unexpected URL.");
            }
        }*/
        
        [Test]
        public async Task TestManageAccountVisibleInNavWhenLoggedIn()
        {
            await RegisterUser();
            // Arrange
            await Page.GotoAsync("https://localhost:5273");
        
            // Act
            await Page.GetByRole(AriaRole.Link, new() { Name = "manage account" }).ClickAsync();
            
            // Assert
            await Expect(Page).ToHaveTitleAsync(new Regex("Profile"));
            await DeleteUser();
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
            await Page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
            await Page.GetByPlaceholder("Please enter your password.").FillAsync("Password123!");
            await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my account" }).ClickAsync();
        }
        
    }
}