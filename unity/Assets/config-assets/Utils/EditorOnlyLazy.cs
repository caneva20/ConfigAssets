using System;

namespace ConfigAssets.Utils {
    internal class EditorOnlyLazy<T> where T : class {
        private readonly Lazy<T> _lazy;

        private T _instance;

        internal T Instance {
            get {
#if UNITY_EDITOR
                return _lazy.Instance;
#else
                return null;
#endif
            }
        }

        internal EditorOnlyLazy(Func<T> creator) {
            _lazy = new Lazy<T>(creator);
        }
    }
}