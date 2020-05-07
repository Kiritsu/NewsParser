using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Prefixes;
using Disqord.Events;
using Disqord.Logging;
using Disqord.Rest.AuditLogs;
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
            var pfxPrvdr = new DefaultPrefixProvider().AddPrefix("k!");
            using (var bot = new DiscordBot(TokenType.Bot, token, pfxPrvdr, new DiscordBotConfiguration
            {
                ProviderFactory = bot => new ServiceCollection()
                    .AddSingleton(bot)
                    .AddSingleton<NewsParserService>()
                    .AddSingleton<HttpClient>()
                    .BuildServiceProvider(),
                CommandServiceConfiguration = new CommandServiceConfiguration
                {
                    CooldownBucketKeyGenerator = GenerateBucketKey,
                    IgnoresExtraArguments = true
                }
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
            if (!(e.Channel is CachedTextChannel gc))
            {
                return;
            }

            if (gc.Guild.Id != 628543142819528714)
            {
                return;
            }

            if (e.NewMessage.Author.IsBot)
            {
                return;
            }

            var embed = new LocalEmbedBuilder()
             .WithColor(Color.DarkRed)
             .WithTitle("Message Updated")
             .AddField("Author", $"{e.NewMessage.Author.Name}#{e.NewMessage.Author.Discriminator}", true)
             .AddField("Date", $"{e.NewMessage.Id.CreatedAt:G}", true)
             .AddField("Channel", $"{(e.Channel as IMentionable).Mention}", true)
             .AddField("Old Content", $"{(e.OldMessage.HasValue ? e.OldMessage.Value.Content : "**Unknown Message**")}")
             .AddField("New Content", $"{e.NewMessage.Content}")
             .Build();

            var tc = e.Client.GetChannel(636992310265118752) as CachedTextChannel;
            await tc.SendMessageAsync(embed: embed);
        }

        private async Task Bot_MessageDeleted(MessageDeletedEventArgs e)
        {
            if (!(e.Channel is CachedTextChannel gc))
            {
                return;
            }

            if (gc.Guild.Id != 628543142819528714)
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

            var embed = new LocalEmbedBuilder()
                .WithColor(Color.DarkRed)
                .WithTitle("Message Deleted")
                .AddField("Author", $"{e.Message.Value.Author.Name}#{e.Message.Value.Author.Discriminator}", true)
                .AddField("Date", $"{e.Message.Value.Id.CreatedAt:G}", true)
                .AddField("Channel", $"{(e.Channel as IMentionable).Mention}", true)
                .AddField("Content", $"{e.Message.Value.Content}");

            var tc = e.Client.GetChannel(636992310265118752) as CachedTextChannel;
            var msg = await tc.SendMessageAsync(embed: embed.Build());

            try
            {
                var auditLogs = await gc.Guild.GetAuditLogsAsync<RestMessagesDeletedAuditLog>();
                var auditLog = auditLogs.FirstOrDefault(x => x.ChannelId == gc.Id && x.TargetId == e.Message.Value.Author.Id);
                if (!(auditLog is null))
                {
                    var usrResp = await auditLog.ResponsibleUser.GetAsync();

                    if ((DateTimeOffset.UtcNow - auditLog.Id.CreatedAt) > TimeSpan.FromMinutes(5))
                    {
                        return;
                    }

                    embed.WithFooter($"Probably deleted by: {usrResp.Name}#{usrResp.Discriminator}");

                    await msg.ModifyAsync(x => x.Embed = embed.Build());
                }
            }
            catch (Exception)
            {

            }
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
            if (!(e.Client is IServiceProvider service))
            {
                Console.WriteLine("SERVICE IS NULL DUMBASS");
                return;
            }
            var newsParser = service.GetService<NewsParserService>();
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
