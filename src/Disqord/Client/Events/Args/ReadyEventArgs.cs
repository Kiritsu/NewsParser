﻿using System.Collections.Generic;
using System.Collections.Immutable;

namespace Disqord.Events
{
    public sealed class ReadyEventArgs : DiscordEventArgs
    {
        public string SessionId { get; }

        public IReadOnlyList<string> Trace { get; }

        internal ReadyEventArgs(DiscordClientBase client, string sessionId, string[] trace) : base(client)
        {
            SessionId = sessionId;
            Trace = trace.ToImmutableArray();
        }
    }
}
