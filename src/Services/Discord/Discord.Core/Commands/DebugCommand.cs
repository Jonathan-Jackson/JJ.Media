using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Threading.Tasks;

namespace Discord.Core.Commands {

    public class DebugCommand {

        [Command("jj-debug-hi")]
        public async Task Hi(CommandContext ctx) {
            await ctx.RespondAsync($"👋 Hi, {ctx.User.Mention}!");
        }

        [Command("jj-random")]
        public async Task Random(CommandContext ctx, int min, int max) {
            var rnd = new Random();
            await ctx.RespondAsync($"🎲 Your random number is: {rnd.Next(min, max)}");
        }
    }
}