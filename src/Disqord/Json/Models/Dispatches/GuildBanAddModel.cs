﻿using Disqord.Serialization.Json;

namespace Disqord.Models.Dispatches
{
    internal sealed class GuildBanAddModel
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; }

        [JsonProperty("user")]
        public UserModel User { get; set; }
    }
}
