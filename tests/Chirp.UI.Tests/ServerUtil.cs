using System.Diagnostics;
using System.Threading.Tasks;
using Azure;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Chirp.UI.Tests
{
    public static class ServerUtil 
    {
        public static async Task<Process> StartServer()
        {
            var serverProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "run --project ../../../../../src/Chirp.Web",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            serverProcess.Start();
            await Task.Delay(10000);

            return serverProcess;

        }

        public static async Task LoginUserAndDeleteUser(string email, string password, IPage page)
        {
            await page.GotoAsync("https://localhost:5273/Identity/Account/Login");

            await page.GetByPlaceholder("name@example.com").FillAsync(email);
            await page.GetByPlaceholder("password").FillAsync(password);
            
            await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            
            var isUserLoggedIn = await page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).IsVisibleAsync();
            if (!isUserLoggedIn)
            {
                return;
            }
            await page.GetByRole(AriaRole.Link, new() { Name = "manage account" }).ClickAsync();
            await page.GetByRole(AriaRole.Link, new() { Name = "Personal data" }).ClickAsync();
            await page.GetByRole(AriaRole.Link, new() { Name = "Delete" }).ClickAsync();
            await page.GetByPlaceholder("Please enter your password.").FillAsync("Testpassword123!");
            await page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
        }
        
        public static async Task RegisterUser(string userName, string email, string password, IPage page)
        {
            await page.GotoAsync("https://localhost:5273/");
            await page.GetByRole(AriaRole.Link, new() { Name = "register" }).ClickAsync();
            await page.GetByPlaceholder("user name").ClickAsync();
            await page.GetByPlaceholder("user name").FillAsync(userName);
            await page.GetByPlaceholder("name@example.com").ClickAsync();
            await page.GetByPlaceholder("name@example.com").FillAsync(email);
            await page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
            await page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
            await page.GetByLabel("Confirm Password").ClickAsync();
            await page.GetByLabel("Confirm Password").FillAsync(password);
            await page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        }
    }
}