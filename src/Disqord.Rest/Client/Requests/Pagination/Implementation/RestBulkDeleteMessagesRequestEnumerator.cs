﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qommon.Collections;

namespace Disqord.Rest
{
    internal sealed class RestBulkDeleteMessagesRequestEnumerator : RestRequestEnumerator<Snowflake>
    {
        private readonly Snowflake _channelId;
        private readonly Snowflake[] _messageIds;

        private int _offset;

        public RestBulkDeleteMessagesRequestEnumerator(RestDiscordClient client,
            Snowflake channelId, Snowflake[] messageIds) : base(client, 100, messageIds.Length)
        {
            _channelId = channelId;
            _messageIds = messageIds;

            _offset = 0;
        }

        protected override async Task<IReadOnlyList<Snowflake>> NextPageAsync(
            IReadOnlyList<Snowflake> previous, RestRequestOptions options = null)
        {
            var amount = Remaining > 100
                ? 100
                : Remaining;
            var segment = new ArraySegment<Snowflake>(_messageIds, _offset, amount);
            _offset += amount;

            if (amount == 1)
                await Client.DeleteMessageAsync(_channelId, segment[0], options).ConfigureAwait(false);
            else
                await Client.DeleteMessagesAsync(_channelId, segment, options).ConfigureAwait(false);

            return new ReadOnlyList<Snowflake>(segment);
        }
    }
}
