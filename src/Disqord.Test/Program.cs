using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Events;
using Disqord.Logging;
using Microsoft.Extensions.DependencyInjection;
using NewsParser.Services;

namespace NewsParser
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        private async Task MainAsync()
        {
            var token = Environment.GetEnvironmentVariable("NOT_QUAHU", EnvironmentVariableTarget.User);
            using (var bot = new DiscordBot(TokenType.Bot, token, new DiscordBotConfiguration
            {
                Prefixes = new[] { "k!" },
                ProviderFactory = bot => new ServiceCollection()
                    .AddSingleton(bot)
                    .AddSingleton<NewsParserService>()
                    .AddSingleton<HttpClient>()
                    .BuildServiceProvider()
            }))
            {
                bot.Ready += Bot_Ready;
                bot.Logger.MessageLogged += Logger_MessageLogged;
                bot.AddModules(Assembly.GetExecutingAssembly());

                await bot.RunAsync();
            }
        }

        private async Task Bot_Ready(ReadyEventArgs e)
        {
            var newsParser = (e.Client as IServiceProvider).GetService<NewsParserService>();
            await newsParser.RunAsync();
        }

        private void Logger_MessageLogged(object sender, MessageLoggedEventArgs e)
        {
            Console.WriteLine(e);
        }
    }
}
