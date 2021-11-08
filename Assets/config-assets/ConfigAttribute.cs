using System;

namespace me.caneva20.ConfigAssets {
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigAttribute : Attribute {
        public string FileName { get; set; }
        public string DisplayName { get; set; }
        public bool EnableProvider { get; set; } = true;
        public SettingsScope Scope { get; set; } = SettingsScope.Project;
        public string[] Keywords { get; set; } = { };

        public static ConfigAttribute Find(Type type) {
            return (ConfigAttribute) GetCustomAttribute(type, typeof(ConfigAttribute));
        }
    }
}
