using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace Discord.Core.Commands {

    public class SubscribeCommand {

        [Command("subscribe")]
        [Description("Mentions you when a new episode for a show has been released. Example: !subscribe [show name]")]
        public async Task Subscribe(CommandContext ctx, params string[] values) {
            string showName = string.Join(' ', values);
            await ctx.RespondAsync($"⛔ Not Yet Implemented.");
        }
    }
}