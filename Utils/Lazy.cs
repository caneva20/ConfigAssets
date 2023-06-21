using System;

namespace ConfigAssets.Utils {
    internal class Lazy<T> {
        private readonly Func<T> _creator;

        private T _instance;

        internal T Instance {
            get {
                _instance ??= _creator();

                return _instance;
            }
        }

        internal Lazy(Func<T> creator) {
            _creator = creator;
        }
    }
}