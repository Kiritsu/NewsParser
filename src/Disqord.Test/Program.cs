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
using Qmmands;

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
            var token = Environment.GetEnvironmentVariable("NOT_QUAHU");
            using (var bot = new DiscordBot(TokenType.Bot, token, new DiscordBotConfiguration
            {
                Prefixes = new[] { "k!" },
                ProviderFactory = bot => new ServiceCollection()
                    .AddSingleton(bot)
                    .AddSingleton<NewsParserService>()
                    .AddSingleton<HttpClient>()
                    .BuildServiceProvider(),
                CommandService = new CommandService(new CommandServiceConfiguration
                {
                    CooldownBucketKeyGenerator = GenerateBucketKey,
                    IgnoresExtraArguments = true
                })
            }))
            {
                bot.Ready += Bot_Ready;
                bot.MessageDeleted += Bot_MessageDeleted;
                bot.MessageUpdated += Bot_MessageUpdated;
                bot.Logger.MessageLogged += Logger_MessageLogged;
                bot.AddModules(Assembly.GetExecutingAssembly());

                await bot.RunAsync();
            }
        }

        private async Task Bot_MessageUpdated(MessageUpdatedEventArgs e)
        {
            if (!(e.Channel is IGuildChannel gc))
            {
                return;
            }

            if (gc.GuildId != 628543142819528714)
            {
                return;
            }

            if (e.NewMessage.Author.IsBot)
            {
                return;
            }

            var embed = new EmbedBuilder()
             .WithColor(Color.DarkRed)
             .WithTitle("Message Updated")
             .AddField("Author", $"{e.NewMessage.Author.Name}#{e.NewMessage.Author.Discriminator}", true)
             .AddField("Date", $"{e.NewMessage.Timestamp:G}", true)
             .AddField("Channel", $"{(e.Channel as IMentionable).Mention}", true)
             .AddField("Old Content", $"{(e.OldMessage.HasValue ? e.OldMessage.Value.Content : "**Unknown Message**")}")
             .AddField("New Content", $"{e.NewMessage.Content}")
             .Build();

            var tc = e.Client.GetChannel(636992310265118752) as ITextChannel;
            await tc.SendMessageAsync(embed: embed);
        }

        private async Task Bot_MessageDeleted(MessageDeletedEventArgs e)
        {
            if (!(e.Channel is IGuildChannel gc))
            {
                return;
            }

            if (gc.GuildId != 628543142819528714)
            {
                return;
            }

            if (!e.Message.HasValue)
            {
                return;
            }

            if (e.Message.Value.Author.IsBot)
            {
                return;
            }

            var embed = new EmbedBuilder()
                .WithColor(Color.DarkRed)
                .WithTitle("Message Deleted")
                .AddField("Author", $"{e.Message.Value.Author.Name}#{e.Message.Value.Author.Discriminator}", true)
                .AddField("Date", $"{e.Message.Value.Timestamp:G}", true)
                .AddField("Channel", $"{(e.Channel as IMentionable).Mention}", true)
                .AddField("Content", $"{e.Message.Value.Content}")
                .Build();

            var tc = e.Client.GetChannel(636992310265118752) as ITextChannel;
            await tc.SendMessageAsync(embed: embed);
        }

        private object GenerateBucketKey(object bucketType, CommandContext ctx)
        {
            var context = ctx as DiscordCommandContext;

            if (bucketType is CooldownBucketType bucket)
            {
                var obj = "";

                switch (bucket)
                {
                    case CooldownBucketType.Guild:
                        obj += context.Guild?.Id ?? context.User.Id;
                        break;
                    case CooldownBucketType.Channel:
                        obj += context.Channel.Id;
                        break;
                    case CooldownBucketType.User:
                        obj += context.User.Id;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown bucket type.");
                }

                return obj;
            }

            throw new InvalidOperationException("Unknown bucket type.");
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

    public enum CooldownBucketType
    {
        Guild,
        Channel,
        User
    }
}
