﻿//using System;
//using Disqord.Models;

//namespace Disqord.Rest.AuditLogs
//{
//    public sealed class RestAuditLogGuildMetadata : RestAuditLogMetadata
//    {
//        public RestAuditLog AuditLog { get; }

//        public AuditLogValue<string> Name { get; }

//        public AuditLogValue<string> IconHash { get; }

//        public AuditLogValue<string> SplashHash { get; }

//        public AuditLogValue<Snowflake> OwnerId { get; }

//        public AuditLogValue<OptionalSnowflakeEntity<RestUser>> Owner { get; }

//        public AuditLogValue<string> VoiceRegionId { get; }

//        public AuditLogValue<Snowflake?> AfkChannelId { get; }

//        public AuditLogValue<int> AfkTimeout { get; }

//        public AuditLogValue<MfaLevel> MfaLevel { get; }

//        public AuditLogValue<VerificationLevel> VerificationLevel { get; }

//        public AuditLogValue<ContentFilterLevel> ContentFilterLevel { get; }

//        public AuditLogValue<DefaultNotificationLevel> DefaultNotificationLevel { get; }

//        public AuditLogValue<string> VanityUrlCode { get; }

//        public AuditLogValue<bool> IsWidgetEnabled { get; }

//        public AuditLogValue<Snowflake?> WidgetChannelId { get; }

//        public AuditLogValue<Snowflake?> SystemChannelId { get; }

//        internal RestAuditLogGuildMetadata(RestAuditLog auditLog, AuditLogModel auditLogModel, AuditLogEntryModel model) : base(auditLog.Client)
//        {
//            AuditLog = auditLog;
//            for (var i = 0; i < model.Changes.Length; i++)
//            {
//                var change = model.Changes[i];
//                switch (change.Key)
//                {
//                    case "name":
//                    {
//                        Name = new AuditLogValue<string>(change);
//                        break;
//                    }

//                    case "icon_hash":
//                    {
//                        IconHash = new AuditLogValue<string>(change);
//                        break;
//                    }

//                    case "splash_hash":
//                    {
//                        SplashHash = new AuditLogValue<string>(change);
//                        break;
//                    }

//                    case "owner_id":
//                    {
//                        OwnerId = new AuditLogValue<Snowflake>(change, x => (ulong) x);
//                        var ownerBeforeModel = Array.Find(auditLogModel.Users, x => x.Id == OwnerId.OldValue.Value);
//                        var ownerAfterModel = Array.Find(auditLogModel.Users, x => x.Id == OwnerId.NewValue.Value);
//                        Owner = new AuditLogValue<OptionalSnowflakeEntity<RestUser>>(
//                            ownerBeforeModel != null
//                                ? new OptionalSnowflakeEntity<RestUser>(new RestUser(Client, ownerBeforeModel))
//                                : new OptionalSnowflakeEntity<RestUser>(OwnerId.OldValue.Value),
//                            ownerAfterModel != null
//                                ? new OptionalSnowflakeEntity<RestUser>(new RestUser(Client, ownerAfterModel))
//                                : new OptionalSnowflakeEntity<RestUser>(OwnerId.NewValue.Value));
//                        break;
//                    }

//                    case "region":
//                    {
//                        VoiceRegionId = new AuditLogValue<string>(change);
//                        break;
//                    }

//                    case "afk_channel_id":
//                    {
//                        AfkChannelId = new AuditLogValue<Snowflake?>(change, x => (ulong?) x);
//                        break;
//                    }

//                    case "afk_timeout":
//                    {
//                        AfkTimeout = new AuditLogValue<int>(change);
//                        break;
//                    }

//                    case "mfa_level":
//                    {
//                        MfaLevel = new AuditLogValue<MfaLevel>(change);
//                        break;
//                    }

//                    case "verification_level":
//                    {
//                        VerificationLevel = new AuditLogValue<VerificationLevel>(change);
//                        break;
//                    }

//                    case "explicit_content_filter":
//                    {
//                        ContentFilterLevel = new AuditLogValue<ContentFilterLevel>(change);
//                        break;
//                    }

//                    case "default_message_notifications":
//                    {
//                        DefaultNotificationLevel = new AuditLogValue<DefaultNotificationLevel>(change);
//                        break;
//                    }

//                    case "vanity_url_code":
//                    {
//                        VanityUrlCode = new AuditLogValue<string>(change);
//                        break;
//                    }

//                    case "widget_enabled":
//                    {
//                        IsWidgetEnabled = new AuditLogValue<bool>(change);
//                        break;
//                    }

//                    case "widget_channel_id":
//                    {
//                        WidgetChannelId = new AuditLogValue<Snowflake?>(change, x => (ulong?) x);
//                        break;
//                    }

//                    case "system_channel_id":
//                    {
//                        SystemChannelId = new AuditLogValue<Snowflake?>(change, x => (ulong?) x);
//                        break;
//                    }
//                }
//            }
//        }
//    }
//}
