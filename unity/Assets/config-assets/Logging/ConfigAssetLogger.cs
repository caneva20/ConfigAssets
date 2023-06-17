using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace me.caneva20.ConfigAssets.Logging {
    public static class ConfigAssetLogger {
        internal static void LogError(Exception exception, string message, Object context) {
            if (!CanLog(LoggingLevel.Error)) {
                return;
            }

            Debug.LogError(FormatMessage($"{exception.Message} | {message}"), context);
        }
        
        internal static void LogError(Exception exception, string message) {
            if (!CanLog(LoggingLevel.Error)) {
                return;
            }

            Debug.LogError(FormatMessage($"{exception.Message} | {message}"));
        }

        internal static void LogError(string message, Object context) {
            if (!CanLog(LoggingLevel.Error)) {
                return;
            }

            Debug.LogError(FormatMessage(message), context);
        }
        
        internal static void LogError(string message) {
            if (!CanLog(LoggingLevel.Error)) {
                return;
            }

            Debug.LogError(FormatMessage(message));
        }

        internal static void LogWarning(string message) {
            if (!CanLog(LoggingLevel.Warning)) {
                return;
            }

            Debug.LogWarning(FormatMessage(message));
        }

        internal static void LogInformation(string message) {
            if (!CanLog(LoggingLevel.Information)) {
                return;
            }

            Debug.Log(FormatMessage(message));
        }

        internal static void LogVerbose(string message) {
            if (!CanLog(LoggingLevel.Verbose)) {
                return;
            }

            Debug.Log(FormatMessage(message));
        }

        private static bool CanLog(LoggingLevel level) {
            var minLevel = (int)Defaults.Instance.LoggingLevel;

            return (int)level <= minLevel;
        }

        private static string FormatMessage(string message) {
            return $"[ConfigAssets] {message}\n**Logging level can be changed in the configurations**\n";
        }
    }
}
