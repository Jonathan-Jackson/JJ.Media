﻿using DSharpPlus.CommandsNext;

namespace Discord.Core.Helpers {

    public static class DSharpCommandExtensions {

        public static CommandsNextModule AddCommand<TCommand>(this CommandsNextModule module)
                where TCommand : class {
            module.RegisterCommands<TCommand>();
            return module;
        }
    }
}