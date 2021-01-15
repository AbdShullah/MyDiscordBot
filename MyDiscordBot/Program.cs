using System.Threading.Tasks;

namespace MyDiscordBot
{
    public static class Program
    {
        public static Task Main(string[] args)
        {
            return Startup.StartAsync();
        }
    }
}