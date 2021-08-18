using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using me.caneva20.ConfigAssets.Loading;
using UnityEditor;
using UnityEditor.Callbacks;

namespace me.caneva20.ConfigAssets.Editor {
    public static class ConfigIndexer {
        [DidReloadScripts]
        private static void OnScriptReload() {
            var configType = typeof(Config);

            var types = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(x => x.GetTypes())
               .Where(x =>
                    !x.IsInterface && !x.IsAbstract && x != configType &&
                    configType.IsAssignableFrom(x)).ToList();

            foreach (var type in types) {
                ConfigLoader.Load(type);
            }

            var guids = AssetDatabase.FindAssets($"t:{typeof(Config)}");

            var configs = guids.Select(guid =>
                AssetDatabase.LoadAssetAtPath<Config>(AssetDatabase.GUIDToAssetPath(guid)));

            SetPreloadList(configs);

            CreateProviders(types);
        }

        private static void SetPreloadList(IEnumerable<Config> configs) {
            var preload = PlayerSettings.GetPreloadedAssets()
               .Where(_ => _ && _ is Config)
               .ToList();

            var except = configs.Except(preload);

            preload.AddRange(except);

            PlayerSettings.SetPreloadedAssets(preload.ToArray());
        }

        private static void CreateProviders(IEnumerable<Type> configs) {
            var builder = new StringBuilder();

            builder.Append("////////////////////////////////////////////////////////////////////////////////\n");
            builder.Append("// This file is auto-generated by ConfigAssets package.\n");
            builder.Append("// Do not modify this file.\n");
            builder.Append("// It will be overwritten next time the generator is run.\n");
            builder.Append("////////////////////////////////////////////////////////////////////////////////\n");
            builder.Append("\n");
            builder.Append("using me.caneva20.ConfigAssets;\n");
            builder.Append("using System.Collections.Generic;\n");
            builder.Append("using UnityEditor;\n");
            builder.Append("using SettingsScope = me.caneva20.ConfigAssets.SettingsScope;\n");
            builder.Append("\n");
            builder.Append("public static class ConfigAssetsSettingsProvider {\n");
            builder.Append(
                "\tprivate static SettingsProvider CreateProvider<T>(string name, SettingsScope scope, IEnumerable<string> keywords) where T : Config<T> {\n");
            builder.Append(
                "\t\treturn new SettingsProvider($\"Config assets/{name}\", scope.ToUnitySettingsScope(), keywords) {\n");
            builder.Append(
                "\t\t\tguiHandler = _ => Editor.CreateEditor(Config<T>.Instance).OnInspectorGUI()\n");
            builder.Append("\t\t};\n");
            builder.Append("\t}\n");
            builder.Append("\n");

            foreach (var configType in configs) {
                var attribute = ConfigAttribute.Find(configType);

                if (attribute?.EnableProvider == false) {
                    continue;
                }

                var name = configType.FullName?.Replace(".", "") ?? $"A{Guid.NewGuid()}";
                var displayName = attribute?.DisplayName ?? configType.Name;
                var scope = $"{nameof(SettingsScope)}.{attribute?.Scope ?? SettingsScope.Project}";
                var keywords =
                    $"new string[] {{{string.Join(", ", (attribute?.Keywords ?? new string[] { }).Select(x => $"\"{x}\""))}}}";

                if (string.IsNullOrEmpty(keywords)) {
                    keywords = "null";
                }

                builder.Append("\t[SettingsProvider]\n");
                builder.Append($"\tpublic static SettingsProvider Create{name}Provider() {{\n");
                builder.Append(
                    $"\t\treturn CreateProvider<{configType.FullName}>(\"{displayName}\", {scope}, {keywords});\n");
                builder.Append("\t}\n");
                builder.Append("\n");
            }

            builder.Append("}\n");

            File.WriteAllText(
                $@"Assets/{Defaults.Instance.CodeGenDirectory}/ConfigAssetsSettingsProvider.cs",
                builder.ToString());

            AssetDatabase.Refresh();
        }
    }
}
