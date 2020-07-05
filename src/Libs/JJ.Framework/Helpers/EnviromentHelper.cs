using System;

namespace JJ.Framework.Helpers {

    public static class EnviromentHelper {

        public static bool TryGetGlobalEnviromentVariable(string key, out string value) {
            value = FindGlobalEnviromentVariable(key);
            return value != null;
        }

        public static string FindGlobalEnviromentVariable(string key)
            => Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process)
            ?? Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine);
    }
}