using NUnit.Framework;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Chirp.UI.Tests
{
    public class EndToEndTest : PageTest
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
        /*
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
            
        }*/
    }
}