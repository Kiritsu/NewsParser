﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qmmands;

namespace Disqord.Bot.Parsers
{
    public sealed class CachedUserParser : TypeParser<CachedUser>
    {
        public static CachedUserParser Instance => _instance ?? (_instance = new CachedUserParser());

        private static CachedUserParser _instance;

        private CachedUserParser()
        { }

        public override ValueTask<TypeParserResult<CachedUser>> ParseAsync(Parameter parameter, string value, CommandContext _)
        {
            var context = (DiscordCommandContext) _;
            if (context.Guild != null)
            {
                var memberParserResult = CachedMemberParser.Instance.ParseAsync(parameter, value, _).Result;
                return memberParserResult.IsSuccessful
                    ? TypeParserResult<CachedUser>.Successful(memberParserResult.Value)
                    : TypeParserResult<CachedUser>.Unsuccessful(memberParserResult.Reason);
            }

            IReadOnlyDictionary<Snowflake, CachedUser> users;
            if (context.Channel is CachedDmChannel dmChannel)
            {
                users = new Dictionary<Snowflake, CachedUser>
                {
                    [dmChannel.Recipient.Id] = dmChannel.Recipient,
                    [context.Bot.CurrentUser.Id] = context.Bot.CurrentUser
                };
            }
            else if (context.Channel is CachedGroupChannel groupChannel)
            {
                var dictionary = groupChannel.Recipients.ToDictionary(x => x.Key, x => x.Value);
                dictionary[context.Bot.CurrentUser.Id] = context.Bot.CurrentUser;
                users = dictionary;
            }
            else
            {
                throw new InvalidOperationException("Unknown channel type.");
            }

            CachedUser user = null;
            if (Discord.TryParseUserMention(value, out var id) || Snowflake.TryParse(value, out id))
                users.TryGetValue(id, out user);

            var values = users.Values;
            if (user == null)
            {
                var hashIndex = value.LastIndexOf('#');
                if (hashIndex != -1 && hashIndex + 5 == value.Length)
                {
                    user = values.FirstOrDefault(x =>
                    {
                        var valueSpan = value.AsSpan();
                        return x.Name.AsSpan().Equals(valueSpan.Slice(0, value.Length - 5), StringComparison.Ordinal) &&
                            x.Discriminator == valueSpan.Slice(hashIndex + 1);
                    });
                }
            }

            if (user == null)
            {
                // TODO custom result type returning the users?
                var matchingUsers = values.Where(x => x.Name == value || x is CachedMember member && member.Nick == value).ToArray();
                if (matchingUsers.Length > 1)
                    return TypeParserResult<CachedUser>.Unsuccessful("Multiple matches found. Mention the user, use their tag or their ID.");

                if (matchingUsers.Length == 1)
                    user = matchingUsers[0];
            }

            return user == null
                ? TypeParserResult<CachedUser>.Unsuccessful("No user found matching the input.")
                : TypeParserResult<CachedUser>.Successful(user);
        }
    }
}