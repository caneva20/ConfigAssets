using ConfigAssets.Infrastructure;
using ConfigAssets.Utils;

namespace ConfigAssets {
    public static class Services {
        public static IConfigLoader ConfigLoader => new Lazy<IConfigLoader>(() => new ConfigLoader()).Instance;
    }
}