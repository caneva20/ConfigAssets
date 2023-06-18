using me.caneva20.ConfigAssets.Logging;
using UnityEngine;

namespace me.caneva20.ConfigAssets {
    [Config(DisplayName = "ConfigAssets settings", GenerateSingleton = true)]
    public partial class Defaults {
        [SerializeField] private LoggingLevel _loggingLevel = LoggingLevel.Warning;
    }
}
