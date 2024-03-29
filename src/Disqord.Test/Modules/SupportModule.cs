using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Events;
using LiteDB;
using NewsParser.Attributes;
using NewsParser.Services;
using Qmmands;

namespace NewsParser.Modules
{
    public enum Lang
    {
        EN,
        FR,
        ES,
        DE,
        RU,
        IT,
        PL,
        TR,
        CZ
    }

    [RunMode(RunMode.Parallel)]
    public sealed class SupportModule : DiscordModuleBase
    {
        private static Dictionary<Lang, Dictionary<string, string>> LangStrings;
        private static Dictionary<Lang, ulong> LangChannels;

        static SupportModule()
        {
            LangChannels = new Dictionary<Lang, ulong>
            {
                [Lang.EN] = 636984863525830686,
                [Lang.CZ] = 636984863525830686,
                [Lang.DE] = 636984863525830686,
                [Lang.ES] = 636984863525830686,
                [Lang.IT] = 636984863525830686,
                [Lang.PL] = 636984863525830686,
                [Lang.RU] = 636984863525830686,
                [Lang.TR] = 636984863525830686,
                [Lang.FR] = 640630987696635927
            };

            LangStrings = new Dictionary<Lang, Dictionary<string, string>>
            {
                [Lang.EN] = new Dictionary<string, string>
                {
                    ["SERVER_KEY"] = "Alright. Now, I will need the server the player you want to report is.",
                    ["CHANNEL_KEY"] = "Next, I need the channel he is in.",
                    ["NICKNAME_KEY"] = "Please give me the nickname of the player you want to report.",
                    ["DETAILS_KEY"] = "Now, I will need further details. What have they done?",
                    ["NEXT_KEY"] = "Thank you for your report. If you have anything else to give, please type: `{0}support {1} <your message>`.",
                    ["CONFIRM_DETAILS_KEY"] = "These details have been sent, thank you.",
                    ["HERE"] = "!"
                },
                [Lang.FR] = new Dictionary<string, string>
                {
                    ["SERVER_KEY"] = "Bien. Sur quel serveur est la personne que vous souhaitez signaler ?",
                    ["CHANNEL_KEY"] = "Ok, maintenant, je vais avoir besoin de son canal.",
                    ["NICKNAME_KEY"] = "Il me faut le pseudo du joueur en question, maintenant.",
                    ["DETAILS_KEY"] = "À présent, je vais avoir besoin de plus de détails. Qu'est-ce-que le joueur en question a fait ?",
                    ["NEXT_KEY"] = "Merci pour votre signalement. Si vous avez des informations supplémentaires à donner, écrivez la commande suivante: `{0}support {1} <votre message>`.",
                    ["CONFIRM_DETAILS_KEY"] = "Ces détails ont été transmis, merci.",
                    ["HERE"] = "@here"
                },
                [Lang.ES] = new Dictionary<string, string>
                {
                    ["SERVER_KEY"] = "De acuerdo. Ahora necesitaré el servidor al que pertenece el jugador que quieres reportar.",
                    ["CHANNEL_KEY"] = "¿En qué canal se encuentra?",
                    ["NICKNAME_KEY"] = "Por favor, dime el nombre del jugador al que quieres reportar.",
                    ["DETAILS_KEY"] = "Finalmente, necesitaré algunos detalles. ¿Qué es lo que ha ocurrido?",
                    ["NEXT_KEY"] = "Gracias por tu reporte. Si quieres señalar algo más, por favor escribe: `{0}support {1} <your message>`.",
                    ["CONFIRM_DETAILS_KEYS"] = "La información ha sido enviada. ¡Gracias!",
                    ["HERE"] = "!"
                },
                [Lang.RU] = new Dictionary<string, string>
                {
                    ["SERVER_KEY"] = "Спасибо. Пожалуйста, укажите, о каком сервере пойдет речь (впишите в чат его номер или название).",
                    ["CHANNEL_KEY"] = "Теперь уточните, на каком канале сейчас находятся игроки, о которых вы хотите сообщить (если это имеет значение).",
                    ["NICKNAME_KEY"] = "Напишите имена персонажей этих игроков.",
                    ["DETAILS_KEY"] = "Теперь напишите как можно более подробно о том, что именно нам нужно проверить. Что делают или сделали эти игроки?",
                    ["NEXT_KEY"] = "Спасибо за ваше сообщение. Если вам есть, что добавить, используйте следующую команду: `{0}support {1} <текст>`.",
                    ["CONFIRM_DETAILS_KEY"] = "Дополнительная информация была добавлена, благодарим вас!",
                    ["HERE"] = "!"
                },
                [Lang.PL] = new Dictionary<string, string>
                {
                    ["SERVER_KEY"] = "Dziękujemy. Podaj proszę serwer, którego dotyczy zgłaszana sprawa.",
                    ["CHANNEL_KEY"] = "Na którym kanale znajduje się zgłaszana postać?",
                    ["NICKNAME_KEY"] = "Podaj proszę nick postaci, którą chcesz zgłosić.",
                    ["DETAILS_KEY"] = "Opisz dokładnie czego dotyczy zgłoszenie.",
                    ["NEXT_KEY"] = "Dziękujemy za zgłoszenie. Jeżeli chcesz poruszyć jeszcze jakąś sprawę użyj komendy: `{0}support {1} <Twoja wiadomość>`.",
                    ["CONFIRM_DETAILS_KEYS"] = "Informacje zostały wysłane, dziękujemy.",
                    ["HERE"] = "!"
                },
                [Lang.IT] = new Dictionary<string, string>
                {
                    ["SERVER_KEY"] = "Ok. Adesso ho bisogno di sapere il server del personaggio che vuoi segnalare.",
                    ["CHANNEL_KEY"] = "Ora il canale in cui si trova.",
                    ["NICKNAME_KEY"] = "Per piacere dimmi il nickname del personaggio che vuoi segnalare.",
                    ["DETAILS_KEY"] = "Ho bisogno di qualche dettaglio in più, cosa ha fatto il personaggio in questione?",
                    ["NEXT_KEY"] = "Grazie per la segnalazione. Se hai altro da aggiungere, scrivi: `{0}support {1} <tuo_messaggio>`.",
                    ["CONFIRM_DETAILS_KEY"] = "La segnalazione è stata inviata, grazie.",
                    ["HERE"] = "!"
                },
                [Lang.DE] = new Dictionary<string, string>
                {
                    ["SERVER_KEY"] = "Alles klar. Nun brauche ich den Server auf dem sich der Spieler befindet, den du melden möchtest.",
                    ["CHANNEL_KEY"] = "In welchem Channel befindet sich dieser Spieler?",
                    ["NICKNAME_KEY"] = "Wie lautet der Charaktername des Spielers, den du melden möchtest?",
                    ["DETAILS_KEY"] = "Jetzt brauche ich die Details. Was genau ist passiert?",
                    ["NEXT_KEY"] = "Danke für deine Meldung. Wenn du noch weitere Informationen hinzufügen möchtest, tu dies mit folgendem Text: `{0}support {1} <deine Nachricht>`.",
                    ["CONFIRM_DETAILS_KEY"] = "Deine Informationen wurden versendet, vielen Dank.",
                    ["HERE"] = "!"
                },
                [Lang.TR] = new Dictionary<string, string>
                {
                    ["SERVER_KEY"] = "Peki. Şimdi sunucuda rapor etmek istediğiniz oyuncunun nickine ihtiyacım olacak..",
                    ["CHANNEL_KEY"] = "Sonra, içinde bulunduğu kanala ihtiyacım var..",
                    ["NICKNAME_KEY"] = "Lütfen bana bildirmek istediğiniz oyuncunun takma adını (nickini) verin.",
                    ["DETAILS_KEY"] = "Şimdi daha fazla ayrıntıya ihtiyacım olacak. Ne yaptılar?",
                    ["NEXT_KEY"] = "Raporun için teşekkürler.Söyleyeceğin başka bir şey varsa, lütfen yazın: `{0}support {1} <your message>`.",
                    ["CONFIRM_DETAILS_KEY"] = "Bu detaylar gönderildi, teşekkür ederim..",
                    ["HERE"] = "!"
                },
            };
        }

        [Command("Support")]
        [DmOnly]
        [Cooldown(1, 5, CooldownMeasure.Minutes, CooldownBucketType.User)]
        public async Task SupportAsync(int id, [Remainder] string details)
        {
            using var db = new LiteDatabase("support.db");

            var reportEntities = db.GetCollection<ReportEntity>("reports");
            reportEntities.EnsureIndex(x => x.Id);

            var report = reportEntities.FindById(id);
            if (report is null)
            {
                return;
            }

            if (report.ReportAuthor != Context.User.Id.ToString())
            {
                return;
            }

            if (report.DetailsGiven)
            {
                return;
            }

            report.DetailsGiven = true;
            reportEntities.Update(report);

            var lang = Lang.EN;
            Enum.TryParse(report.Community, true, out lang);

            var supportChannel = Context.Bot.GetChannel(LangChannels[lang]);
            var embed = new LocalEmbedBuilder()
                .WithColor(Color.DarkOrange)
                .WithTitle($"Support request in community **{report.Community}** (ID#{report.Id}) has been updated")
                .WithDescription($"This request was created by `{Context.User.Name.Replace("`", "")}#{Context.User.Discriminator}`.")
                .AddField("Server", report.Server, true)
                .AddField("Channel", report.Channel, true)
                .AddField("Username", report.Nickname, true)
                .AddField("Details", report.Details)
                .AddField("Few more details", details);

            await (supportChannel as CachedTextChannel).SendMessageAsync(embed: embed.Build());

            await ReplyAsync(LangStrings[lang]["CONFIRM_DETAILS_KEY"]);
        }

        [Command("Support")]
        [Cooldown(1, 5, CooldownMeasure.Minutes, CooldownBucketType.User)]
        public async Task SupportAsync()
        {
            using var db = new LiteDatabase("support.db");

            var reportEntities = db.GetCollection<ReportEntity>("reports");
            reportEntities.EnsureIndex(x => x.Id);

            CachedMessage community;
            var lang = Lang.EN;
            var antiSpam = 0;
            do
            {
                if (antiSpam > 3)
                {
                    return;
                }

                try
                {
                    await Context.User.SendMessageAsync("Hello! To start with, please give me your community: EN, FR, ES, DE, RU, IT, PL, TR, CZ");

                    try
                    {
                        await Context.Message.DeleteAsync();
                    }
                    catch (Exception)
                    {
                        //fuck permissions.
                    }
                }
                catch (Exception)
                {
                    var message = await ReplyAsync($"{Context.User.Mention}: It seems you have your private messages disabled. Please enable them then retry.");
                    
                    await Task.Delay(10000);

                    try
                    {
                        await message.DeleteAsync();
                        await Context.Message.DeleteAsync();
                    }
                    catch (Exception)
                    {
                        //fuck permissions.
                    }

                    return;
                }

                community = await NextMessageAsync(Context.User, Context.User.DmChannel);

                if (community == null)
                {
                    return;
                }

                antiSpam++;
            }
            while (!Enum.TryParse(community.Content, true, out lang));

            var language = LangStrings.GetValueOrDefault(lang, LangStrings[Lang.EN]);

            await Context.User.SendMessageAsync(language["SERVER_KEY"]);
            var server = await NextMessageAsync(Context.User, Context.User.DmChannel);

            if (server == null)
            {
                return;
            }

            await Context.User.SendMessageAsync(language["CHANNEL_KEY"]);
            var channel = await NextMessageAsync(Context.User, Context.User.DmChannel);

            if (channel == null)
            {
                return;
            }

            await Context.User.SendMessageAsync(language["NICKNAME_KEY"]);
            var username = await NextMessageAsync(Context.User, Context.User.DmChannel);

            if (username == null)
            {
                return;
            }

            await Context.User.SendMessageAsync(language["DETAILS_KEY"]);
            var reason = await NextMessageAsync(Context.User, Context.User.DmChannel);

            if (reason == null)
            {
                return;
            }

            var report = new ReportEntity
            {
                Channel = channel.Content,
                Community = community.Content,
                Details = reason.Content,
                Nickname = username.Content,
                ReportAuthor = $"{Context.User.Id}",
                Server = server.Content,
                DetailsGiven = false
            };
            reportEntities.Insert(report);

            await Context.User.SendMessageAsync(string.Format(language["NEXT_KEY"], Context.Prefix, report.Id));

            var supportChannel = Context.Bot.GetChannel(LangChannels[lang]);
            var embed = new LocalEmbedBuilder()
                .WithColor(Color.DarkBlue)
                .WithTitle($"Support request in community **{report.Community}** (ID#{report.Id})")
                .WithDescription($"This request has been created by `{Context.User.Name.Replace("`", "")}#{Context.User.Discriminator}`.")
                .AddField("Server", report.Server, true)
                .AddField("Channel", report.Channel, true)
                .AddField("Username", report.Nickname, true)
                .AddField("Details", report.Details);

            await (supportChannel as CachedTextChannel).SendMessageAsync(language["HERE"], embed: embed.Build());
        }

        public async Task<CachedMessage> NextMessageAsync(CachedUser user, CachedChannel channel)
        {
            var timeout = TimeSpan.FromMinutes(5);

            var eventTrigger = new TaskCompletionSource<CachedMessage>();
            var cancelTrigger = new TaskCompletionSource<bool>();

            Task Handler(MessageReceivedEventArgs e)
            {
                if (e.Message.Channel.Id == channel.Id && e.Message.Author.Id == user.Id)
                {
                    eventTrigger.SetResult(e.Message);
                }

                return Task.CompletedTask;
            }

            Context.Bot.MessageReceived += Handler;

            var trigger = eventTrigger.Task;
            var cancel = cancelTrigger.Task;
            var delay = Task.Delay(timeout);
            var task = await Task.WhenAny(trigger, delay, cancel);

            Context.Bot.MessageReceived -= Handler;

            return task == trigger ? await trigger : null;
        }
    }
}

