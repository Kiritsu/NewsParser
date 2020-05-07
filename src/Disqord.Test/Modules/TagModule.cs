using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disqord.Bot;
using LiteDB;
using NewsParser;
using NewsParser.Services;
using Qmmands;

namespace Disqord.Test.Modules
{
    [Group("tag")]
    public sealed class TagModule : DiscordModuleBase
    {
        [Command]
        [Cooldown(1, 10, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task TagAsync(string name, CachedMember member = null)
        {
            var lowerName = name.ToLower();

            using var db = new LiteDatabase("tags.db");
            var tags = db.GetCollection<TagEntity>("tags");

            var tag = tags.FindOne(x => x.TagName.ToLower() == lowerName);
            if (tag is null)
            {
                await ReplyAsync($"Tag `{lowerName}` not found.");
            }
            else if (member is null)
            {
                await ReplyAsync($"{tag.TagContent}");
            }
            else
            {
                await ReplyAsync($"{member.Mention}: {tag.TagContent}");
            }
        }

        [Command("create")]
        [RequireMemberGuildPermissions(Permission.KickMembers)]
        public async Task CreateAsync(string name, [Remainder] string content)
        {
            var lowerName = name.ToLower();
            if (lowerName == "create" || lowerName == "delete" || lowerName == "list")
            {
                await ReplyAsync("Invalid name tag.");
            }

            using var db = new LiteDatabase("tags.db");
            var tags = db.GetCollection<TagEntity>("tags");

            var tag = tags.FindOne(x => x.TagName.ToLower() == lowerName);
            if (!(tag is null))
            {
                await ReplyAsync($"Tag `{lowerName}` already exists.");
            }
            else
            {
                tags.Insert(new TagEntity
                {
                    TagContent = content,
                    TagName = lowerName
                });

                await ReplyAsync($"Tag `{lowerName}` added.");
            }
        }

        [Command("delete")]
        [RequireMemberGuildPermissions(Permission.KickMembers)]
        public async Task DeleteAsync(string name)
        {
            var lowerName = name.ToLower();
            if (lowerName == "create" || lowerName == "delete" || lowerName == "list")
            {
                await ReplyAsync("Invalid name tag.");
            }

            using var db = new LiteDatabase("tags.db");
            var tags = db.GetCollection<TagEntity>("tags");

            var tag = tags.FindOne(x => x.TagName.ToLower() == lowerName);
            if (tag is null)
            {
                await ReplyAsync($"Tag `{lowerName}` not found.");
            }
            else
            {
                tags.Delete(tag.Id);
                await ReplyAsync($"Tag `{lowerName}` deleted.");
            }
        }

        [Command("list")]
        [Cooldown(1, 2, CooldownMeasure.Minutes, CooldownBucketType.Channel)]
        public async Task ListAsync()
        {
            using var db = new LiteDatabase("tags.db");
            var tags = db.GetCollection<TagEntity>("tags");

            await ReplyAsync(string.Join(", ", tags.FindAll().Select(x => $"`{x.TagName}`")));
        }
    }
}
