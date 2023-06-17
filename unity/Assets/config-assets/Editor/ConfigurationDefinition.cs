using System;

namespace me.caneva20.ConfigAssets.Editor {
    internal class ConfigurationDefinition {
        public Type Type { get; set; }
        public ConfigAttribute Attribute { get; set; }
        
        public bool HasNamespace { get; set; }
        public bool IsPartial { get; set; }

        public bool IsValid => HasNamespace && IsPartial;

        public override string ToString() {
            return $"{{type={Type.FullName}; HasNamespace={HasNamespace}; IsPartial={IsPartial}; IsValid={IsValid}}}";
        }
    }
}
