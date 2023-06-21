namespace ConfigAssets.Sourcegen.Utils {
    public static class StringExtensions {
        public static string Sanitize(this string str) {
            return str.Replace(".", "");
        }
    }
}