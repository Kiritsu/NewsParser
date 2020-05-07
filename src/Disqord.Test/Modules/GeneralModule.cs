using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Disqord;
using Disqord.Bot;
using HtmlAgilityPack;
using LiteDB;
using NewsParser.Services;
using Qmmands;

namespace NewsParser
{
    [GuildOnly]
    [RequireMemberGuildPermissions(Permission.Administrator, Group = "God")]
    [RequireUser(550635683258171413, Group = "God")]
    public sealed class GeneralModule : DiscordModuleBase
    {
        private readonly NewsParserService _newsParser;
        private readonly HttpClient _http;

        public GeneralModule(NewsParserService newsParser, HttpClient http)
        {
            _newsParser = newsParser;
            _http = http;
        }

        [Command("getlastpost")]
        public async Task GetPosts(string url)
        {
            try
            {
                var html = new HtmlWeb();
                var content = html.Load(url);
                
                var nodes = content.DocumentNode.SelectNodes("//div[@class='messageText']");

                if (nodes is null)
                {
                    Console.WriteLine("Current node doesn't have `//div[@class='messageText']`");
                    return;
                }
                
                await ReplyAsync(nodes.Last().InnerText);
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
                await ReplyAsync(e.StackTrace);
            }
        }

        [Command("stats")]
        public async Task StatsAsync()
        {
            var str = "```" + string.Join("\n", Context.Guild.Roles.Values.Where(x => !x.IsManaged && !x.IsDefault && !x.Color.HasValue).OrderByDescending(x => x.Members.Count).Select(x => $"- {x.Name}: {x.Members.Count} members")) +
                $"\n\nTotal members: {Context.Guild.Members.Count} ({Context.Guild.Members.Values.Count(x => x.Roles.Count == 1)} members don't have any role)" +
                $"\nChannels: {Context.Guild.Channels.Count} ({Context.Guild.TextChannels.Count} text channels, {Context.Guild.CategoryChannels.Count} categories)" +
                "```";
            
            await ReplyAsync(str, mentions: LocalMentions.None);
        }

        [Command("ping")]
        public Task PingAsync()
        {
            return ReplyAsync("Pong!");
        }

        [Command("add")]
        public async Task AddAsync(CachedGuildChannel channel, string feedUrl)
        {
            using var db = new LiteDatabase("news_parser.db");

            var rssEntities = db.GetCollection<RSSEntity>("rsss");
            rssEntities.EnsureIndex(x => x.Id);

            if (!(channel is CachedTextChannel))
            {
                await ReplyAsync($"The given channel `{channel.Name}` is not a text channel.");
                return;
            }

            if (!await IsValidRSSFeedAsync(feedUrl))
            {
                await ReplyAsync("The given feed url is not valid.");
                return;
            }

            var entity = new RSSEntity
            {
                ChannelId = channel.Id,
                RSSUrl = feedUrl
            };

            rssEntities.Insert(entity);

            _newsParser.Start(entity);

            await ReplyAsync($"The given feed url has been scheduled on channel {(channel as IMentionable).Mention}.");
        }

        [Command("remove")]
        public Task RemoveAsync(int id)
        {
            using var db = new LiteDatabase("news_parser.db");

            var rssEntities = db.GetCollection<RSSEntity>("rsss");
            rssEntities.EnsureIndex(x => x.Id);

            var entity = rssEntities.FindById(id);
            if (entity == null)
            {
                return ReplyAsync("The given id is not valid. Has-it already been removed?");
            }

            rssEntities.Delete(id);

            _newsParser.Stop(id);

            var channel = Context.Bot.GetChannel(entity.ChannelId);

            return ReplyAsync($"The feed `{entity.RSSUrl}` is no longer being tracked on channel {(channel != null ? (channel as IMentionable).Mention : "#deleted-channel")}.");
        }

        [Command("list")]
        public Task ListAsync()
        {
            using var db = new LiteDatabase("news_parser.db");

            var rssEntities = db.GetCollection<RSSEntity>("rsss");
            rssEntities.EnsureIndex(x => x.Id);

            var builder = new LocalEmbedBuilder()
                .WithColor(Color.Aqua);

            foreach (var group in rssEntities.FindAll().GroupBy(x => x.ChannelId))
            {
                var channel = Context.Bot.GetChannel(group.Key);

                if (channel is null)
                {
                    rssEntities.Delete(x => x.ChannelId == group.Key);
                    continue;
                }

                builder.AddField($"{channel.Name}", string.Join("\n", group.Select(x => $"`[{x.Id}]`: {x.RSSUrl}")));
            }

            if (builder.Fields.Count == 0)
            {
                return ReplyAsync(embed: builder.WithDescription("There are no RSS feed being tracked right now.").Build());
            }

            return ReplyAsync(embed: builder.Build());
        }

        private async Task<bool> IsValidRSSFeedAsync(string feedUrl)
        {
            try
            {
                var xmlFile = await _http.GetStringAsync(feedUrl);

                var doc = new XmlDocument();
                doc.LoadXml(xmlFile);

                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }
    }
}
