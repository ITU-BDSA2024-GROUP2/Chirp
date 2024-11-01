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

        [SetUp]
        public async Task Setup()
        {
            
        }

        [TearDown]
        public void Cleanup()
        {
            
        }
        
        [Test]
        public async Task TestTitle()
        {   
            //Arrange
            
            //Act
            await Page.GotoAsync("http://localhost:5273");
            
            //Assert
            await Expect(Page).ToHaveTitleAsync(new Regex("Chirp"));
            
        }
        
        [Test]
        public async Task TestClickOnUser()
        {
            //Arrange
            await Page.GotoAsync("http://localhost:5273");
            
            //Act
            var author = await Page.Locator("p a").First.InnerTextAsync();
            var locator = Page.Locator("p a").First;
            
            //Assert
            await Expect(locator).ToHaveAttributeAsync("href", $"/{author}");
            
        }
        
        [Test]
        public async Task TestClickLoginAndRegister()
        {
            //Arrange
            await Page.GotoAsync("http://localhost:5273");
            
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
            await Page.GotoAsync("http://localhost:5273/Identity/Account/Login");
            
            //Act
            var locator = Page.GetByRole(AriaRole.Link, new() { Name = "Icon1" });
            
            //Assert
            await Expect(locator).ToHaveAttributeAsync("href", "/");
            
        }
        
        [Test]
        public async Task TestLoginForm()
        {
            //Arrange
            await Page.GotoAsync("http://localhost:5273/Identity/Account/Login");
            
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
            await Page.GotoAsync("http://localhost:5273/Identity/Account/Register");
            
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
        
        
        /*[Test]
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
            await Expect(validationMessage).ToBeVisibleAsync(); 
        }
        
        [Test]	
        public async Task TestValidPassword()
        {
            await Page.GotoAsync("http://localhost:5273/Identity/Account/Register");
            
            var passwordInput = Page.Locator("input[name='Input.Password']");
            
            await passwordInput.FillAsync("valid.password@example.com");
            
        }*/
        
    }
}