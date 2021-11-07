using System.Collections.Generic;

namespace me.caneva20.ConfigAssets.Editor {
    partial class SettingsProviderGenerator {
        public IEnumerable<ProviderDefinition> Providers { get; }

        public SettingsProviderGenerator(IEnumerable<ProviderDefinition> providers) {
            Providers = providers;
        }
    }
}
