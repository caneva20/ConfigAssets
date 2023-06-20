namespace ConfigAssets.Package.Models {
    public class PackageDefinition {
        public string Name { get; }
        public string Version { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string Author { get; set; }

        public PackageDefinition(string name, string version, string displayName, string description) {
            Name = name;
            Version = version;
            DisplayName = displayName;
            Description = description;
        }
    }
}