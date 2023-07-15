using ConfigAssets.Infrastructure;

namespace ConfigAssets {
    public static class Services {
        public static readonly IConfigLoader ConfigLoader = new ConfigLoader();
    }
}