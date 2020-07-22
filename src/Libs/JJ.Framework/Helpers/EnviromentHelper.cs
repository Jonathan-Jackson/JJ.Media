using System;
using System.Text.Json;

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

        public static TOutput FindGlobalEnviromentJsonVariable<TOutput>(string key) {
            var json = FindGlobalEnviromentVariable(key);

            if (json == null)
                return default;
            else
                return JsonSerializer.Deserialize<TOutput>(json);
        }

        public static string GetSetting(string settingName, string fallbackValue = "", bool allowEmpty = false) {
            return FindGlobalEnviromentVariable(settingName)
                ?? (allowEmpty || !string.IsNullOrWhiteSpace(fallbackValue) ? fallbackValue : throw new ApplicationException($"{settingName} NOT SPECIFIED. USE AN ENVIROMENT VAR."));
        }
    }
}