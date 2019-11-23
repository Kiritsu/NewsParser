using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Xml;
using Disqord;
using Disqord.Bot;
using LiteDB;

namespace NewsParser.Services
{
    public sealed class NewsParserService
    {
        private readonly DiscordBot _bot;

        private readonly HttpClient _http;
        private readonly Dictionary<int, IDisposable> _intervals;

        public NewsParserService(DiscordBot bot, HttpClient http)
        {
            _bot = bot;
            _http = http;

            _intervals = new Dictionary<int, IDisposable>();
        }

        public async Task RunAsync()
        {
            using var db = new LiteDatabase("news_parser.db");

            var rssEntities = db.GetCollection<RSSEntity>("rsss");
            rssEntities.EnsureIndex(x => x.Id);

            foreach (var entity in rssEntities.FindAll())
            {
                Start(entity);
            }
        }

        public void Stop(int id)
        {
            if (_intervals.TryGetValue(id, out var disposable))
            {
                disposable.Dispose();
            }
        }

        public void Start(RSSEntity entity)
        {
            _intervals.Add(entity.Id, Observable.Interval(TimeSpan.FromMinutes(1)).Subscribe(async _ =>
            {
                try
                {
                    var channel = _bot.GetChannel(entity.ChannelId);

                    var xmlFile = await _http.GetStringAsync(entity.RSSUrl);

                    var doc = new XmlDocument();
                    doc.LoadXml(xmlFile);

                    var topics = doc.GetElementsByTagName("item");
                    if (topics.Count == 0)
                    {
                        return;
                    }

                    var latest = topics[0];
                    var childs = latest.ChildNodes;
                    var topic = new TopicEntity
                    {
                        RSSEntityId = entity.Id
                    };

                    foreach (XmlElement child in childs)
                    {
                        switch (child.Name)
                        {
                            case "title":
                                topic.Name = child.InnerText;
                                break;
                            case "link":
                                topic.Url = child.InnerText;
                                break;
                            case "dc:creator":
                                topic.Author = child.InnerText;
                                break;
                            case "pubDate":
                                topic.CreatedAt = child.InnerText;
                                break;
                        }
                    }

                    using var db = new LiteDatabase("news_parser.db");

                    var topicEntities = db.GetCollection<TopicEntity>("topics");
                    topicEntities.EnsureIndex(x => x.Id);

                    if (topicEntities.FindAll().ToList().Any(x => x.Name == topic.Name && x.CreatedAt == topic.CreatedAt))
                    {
                        return;
                    }

                    topicEntities.Insert(topic);

                    await (channel as ITextChannel).SendMessageAsync(
                        embed: new LocalEmbedBuilder()
                            .WithAuthor(new LocalEmbedAuthorBuilder()
                                            .WithName(topic.Name)
                                            .WithUrl(topic.Url))
                            .WithColor(Color.Aqua)
                            .WithDescription($"A new thread has been created by `{topic.Author}`." +
                                             $"\n\n[Click here]({topic.Url}) to see the entire thread.")
                            .WithFooter($"Created on {topic.CreatedAt}")
                            .Build());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Parser br0ke but nvm lets retry");
                }
            }));
        }
    }
}
