using NUnit.Framework;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;

namespace Chirp.UI.Tests
{
    public class Tests : PageTest
    {
        private Process _serverProcess;

        [SetUp]
        public async Task Setup()
        {
            _serverProcess = await ServerUtil.StartServer();
        }

        [TearDown]
        public void Cleanup()
        {
            _serverProcess.Kill();
            _serverProcess.Dispose();
        }

        [Test]
        public async Task Test1()
        {
            await Page.GotoAsync("http://localhost:5273");

            Assert.IsTrue(await Page.IsVisibleAsync("h1"));
        }
    }
}