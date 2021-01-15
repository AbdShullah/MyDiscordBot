using System.Threading.Tasks;

namespace MyDiscordBot
{
    internal static class Program
    {
        private static Task Main(string[] args)
        {
            return Startup.StartAsync();
        }
    }
}