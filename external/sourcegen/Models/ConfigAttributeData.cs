namespace ConfigAssets.Sourcegen.Models {
    public class ConfigAttributeData {
        public string FileName { get; set; }
        public string DisplayName { get; set; }
        public bool EnableProvider { get; set; } = true;
        public int Scope { get; set; } = 1;
        public string[] Keywords { get; set; } = { };

        public bool GenerateSingleton { get; set; }
    }
}