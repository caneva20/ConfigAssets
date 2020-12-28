using me.caneva20.ConfigAssets.Loading;
using UnityEngine;

namespace me.caneva20.ConfigAssets {
    public class Config : ScriptableObject { }

    public class Config<T> : Config where T : Config {
        private static T _instance;

        public static T Instance {
            get {
                if (_instance == null) {
                    _instance = ConfigLoader.Load<T>();
                }

                return _instance;
            }
        }
    }
}