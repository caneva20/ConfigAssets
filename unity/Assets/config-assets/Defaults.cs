using ConfigAssets.Logging;
using UnityEngine;

namespace ConfigAssets {
    [Config(DisplayName = "ConfigAssets settings", GenerateSingleton = true)]
    public partial class Defaults {
        [SerializeField] private LoggingLevel _loggingLevel = LoggingLevel.Warning;
    }
}
