using System;

namespace ConfigAssets {
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigAttribute : Attribute {
        public string DisplayName { get; set; }
        public bool EnableProvider { get; set; } = true;
        public SettingsScope Scope { get; set; } = SettingsScope.Project;
        public string[] Keywords { get; set; } = { };

        public bool GenerateSingleton { get; set; }
    }
}
