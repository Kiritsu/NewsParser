using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Qmmands;

namespace NewsParser.Attributes
{
    public sealed class DmOnlyAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext _)
        {
            var context = _ as DiscordCommandContext;
            return context.Guild == null && context.Channel is IDmChannel
                ? CheckResult.Successful
                : CheckResult.Unsuccessful("This must be executed in a guild.");
        }
    }
}
