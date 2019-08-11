using UnityEngine;

namespace caneva20.ConfigAssets {
    public class Config<T> : ScriptableObject where T : ScriptableObject {
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