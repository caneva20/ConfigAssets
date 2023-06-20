using System;

namespace ConfigAssets.Utils {
    internal class EditorOnlyLazy<T> where T : class {
        private readonly Func<T> _creator;

        private T _instance;

        internal T Instance {
            get {
                _instance ??= Get();

                return _instance;
            }
        }

        internal EditorOnlyLazy(Func<T> creator) {
            _creator = creator;
        }

        private T Get() {
#if UNITY_EDITOR
            return _creator();
#else
                return null;
#endif
        }
    }
}