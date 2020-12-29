using System;

namespace me.caneva20.ConfigAssets {
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigAttribute : Attribute {
        public string FileName { get; set; }
        public string DisplayName { get; set; }
        public bool EnableProvider { get; set; } = true;

        public static ConfigAttribute Find<T>() where T : Config {
            return Find(typeof(T));
        }

        public static ConfigAttribute Find(Type type) {
            return (ConfigAttribute) GetCustomAttribute(type, typeof(ConfigAttribute));
        }
    }
}
