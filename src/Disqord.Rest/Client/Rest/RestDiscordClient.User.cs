﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Disqord.Rest
{
    public partial class RestDiscordClient : IRestDiscordClient
    {
        public async Task<RestCurrentUser> GetCurrentUserAsync(RestRequestOptions options = null)
        {
            var model = await ApiClient.GetCurrentUserAsync(options).ConfigureAwait(false);
            return new RestCurrentUser(this, model);
        }

        public async Task<RestUser> GetUserAsync(Snowflake userId, RestRequestOptions options = null)
        {
            try
            {
                var model = await ApiClient.GetUserAsync(userId, options).ConfigureAwait(false);
                return new RestUser(this, model);
            }
            catch (DiscordHttpException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<RestCurrentUser> ModifyCurrentUserAsync(Action<ModifyCurrentUserProperties> action, RestRequestOptions options = null)
        {
            var properties = new ModifyCurrentUserProperties();
            action(properties);
            var model = await ApiClient.ModifyCurrentUserAsync(properties, options).ConfigureAwait(false);
            var user = new RestCurrentUser(this, model);
            if (!CurrentUser.HasValue)
                CurrentUser.SetValue(user);
            else
                CurrentUser.Value.Update(model);

            return user;
        }

        public RestRequestEnumerable<RestPartialGuild> GetGuildsEnumerable(int limit, RetrievalDirection direction = RetrievalDirection.Before, Snowflake? startFromId = null)
            => new RestRequestEnumerable<RestPartialGuild>(new RestGuildsRequestEnumerator(this, limit, direction, startFromId));

        public Task<IReadOnlyList<RestPartialGuild>> GetGuildsAsync(int limit = 100, RetrievalDirection direction = RetrievalDirection.Before, Snowflake? startFromId = null, RestRequestOptions options = null)
        {
            if (limit == 0)
                return Task.FromResult<IReadOnlyList<RestPartialGuild>>(ImmutableArray<RestPartialGuild>.Empty);

            if (limit <= 100)
                return InternalGetGuildsAsync(limit, direction, startFromId);

            var enumerable = GetGuildsEnumerable(limit, direction, startFromId);
            return enumerable.FlattenAsync(options);
        }

        internal async Task<IReadOnlyList<RestPartialGuild>> InternalGetGuildsAsync(int limit = 100, RetrievalDirection direction = RetrievalDirection.Before, Snowflake? startFromId = null, RestRequestOptions options = null)
        {
            var models = await ApiClient.GetCurrentUserGuildsAsync(limit, direction, startFromId, options).ConfigureAwait(false);
            return models.Select(x => new RestPartialGuild(this, x)).ToImmutableArray();
        }

        public Task LeaveGuildAsync(Snowflake guildId, RestRequestOptions options = null)
            => ApiClient.LeaveGuildAsync(guildId, options);

        public async Task<IReadOnlyList<RestPrivateChannel>> GetPrivateChannelsAsync(RestRequestOptions options = null)
        {
            var models = await ApiClient.GetUserDmsAsync(options).ConfigureAwait(false);
            return models.Select(x => RestPrivateChannel.Create(this, x)).ToImmutableArray();
        }

        public async Task<RestDmChannel> CreateDmChannelAsync(Snowflake userId, RestRequestOptions options = null)
        {
            var model = await ApiClient.CreateDmAsync(userId, options).ConfigureAwait(false);
            return new RestDmChannel(this, model);
        }

        public async Task<IReadOnlyList<RestConnection>> GetConnectionsAsync(RestRequestOptions options = null)
        {
            var models = await ApiClient.GetUserConnectionsAsync(options).ConfigureAwait(false);
            return models.Select(x => new RestConnection(this, x)).ToImmutableArray();
        }
    }
}
