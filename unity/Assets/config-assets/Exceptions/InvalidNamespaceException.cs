using System;

namespace me.caneva20.ConfigAssets.Exceptions {
    public class InvalidNamespaceException : Exception {
        public Type Type { get; set; }

        public InvalidNamespaceException(string message, Type targetType) : base(message) { }
    }
}
