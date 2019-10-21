﻿using System;
using Disqord.Serialization.Json;

namespace Disqord.Models
{
    internal sealed class MessageModel
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("channel_id")]
        public ulong ChannelId { get; set; }

        [JsonProperty("guild_id", NullValueHandling = NullValueHandling.Ignore)]
        public ulong? GuildId { get; set; }

        [JsonProperty("author")]
        public Optional<UserModel> Author { get; set; }

        [JsonProperty("member")]
        public Optional<MemberModel> Member { get; set; }

        [JsonProperty("content")]
        public Optional<string> Content { get; set; }

        [JsonProperty("timestamp")]
        public Optional<DateTimeOffset> Timestamp { get; set; }

        [JsonProperty("edited_timestamp")]
        public Optional<DateTimeOffset?> EditedTimestamp { get; set; }

        [JsonProperty("tts")]
        public Optional<bool> Tts { get; set; }

        [JsonProperty("mention_everyone")]
        public Optional<bool> MentionEveryone { get; set; }

        [JsonProperty("mentions")]
        public Optional<UserModel[]> Mentions { get; set; }

        [JsonProperty("mention_roles")]
        public Optional<ulong[]> RoleMentions { get; set; }

        [JsonProperty("attachments")]
        public Optional<AttachmentModel[]> Attachments { get; set; }

        [JsonProperty("embeds")]
        public Optional<EmbedModel[]> Embeds { get; set; }

        [JsonProperty("reactions")]
        public Optional<ReactionModel[]> Reactions { get; set; }

        [JsonProperty("nonce")]
        public Optional<ulong?> Nonce { get; set; }

        [JsonProperty("pinned")]
        public Optional<bool> Pinned { get; set; }

        [JsonProperty("webhook_id")]
        public Optional<ulong?> WebhookId { get; set; }

        [JsonProperty("type")]
        public MessageType Type { get; set; }

        //[JsonProperty("activity", NullValueHandling = NullValueHandling.Ignore)]
        //public string Title { get; set; }

        //[JsonProperty("application", NullValueHandling = NullValueHandling.Ignore)]
        //public string Title { get; set; }
    }
}
