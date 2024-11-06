using System.Diagnostics;
using System.Threading.Tasks;

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
    }
}