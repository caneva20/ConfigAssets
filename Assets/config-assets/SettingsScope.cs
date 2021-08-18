//DISCLAIMER
//THIS IS A COPY OF UnityEditor.SettingsScope
//It is only provided here as a way to avoid compilation errors due to using ´UnityEditor` namespace
//This is intended to be compilable at all time (this include release builds) and will work the exact same as Unity's version

using System;

namespace me.caneva20.ConfigAssets {
    /// <summary>
    ///   <para>Sets the scope of a SettingsProvider. The Scope determines where it appears in the UI. For example, whether it appears with the Project settings in the Settings window, or in the Preferences window, or in both windows.</para>
    /// </summary>
    public enum SettingsScope {
        /// <summary>
        ///   <para>The SettingsProvider appears only in the Preferences window.</para>
        /// </summary>
        User,

        /// <summary>
        ///   <para>The SettingsProvider appears only in the Project Settings window.</para>
        /// </summary>
        Project,
    }

    public static class SettingsScopeExtensions {
    #if UNITY_EDITOR
        public static UnityEditor.SettingsScope ToUnitySettingsScope(this SettingsScope scope) {
            return scope switch {
                SettingsScope.Project => UnityEditor.SettingsScope.Project,
                SettingsScope.User => UnityEditor.SettingsScope.User,
                _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
            };
        }
    #endif
    }
}
