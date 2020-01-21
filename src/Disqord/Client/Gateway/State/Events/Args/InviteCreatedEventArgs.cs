﻿using System;

namespace Disqord.Events
{
    public sealed class InviteCreatedEventArgs : DiscordEventArgs
    {
        public CachedGuild Guild { get; }

        public OptionalSnowflakeEntity<CachedChannel> Channel { get; }

        public CachedUser Inviter { get; }

        public string Code { get; }

        public bool IsTemporary { get; }

        public int MaxUses { get; }

        public int MaxAge { get; }

        public DateTimeOffset CreatedAt { get; }

        internal InviteCreatedEventArgs(
            DiscordClientBase client,
            CachedGuild guild,
            OptionalSnowflakeEntity<CachedChannel> channel,
            CachedUser inviter,
            string code,
            bool isTemporary,
            int maxUses,
            int maxAge,
            DateTimeOffset createdAt) : base(client)
        {
            Guild = guild;
            Channel = channel;
            Inviter = inviter;
            Code = code;
            IsTemporary = isTemporary;
            MaxUses = maxUses;
            MaxAge = maxAge;
            CreatedAt = createdAt;
        }
    }
}
