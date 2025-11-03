using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace GeotuneActivity
{
    [ConfigFile(IndentOutput: true, SharedConfigLocation: true)]
    internal class GeotuneActivityConfig : SingletonOptions<GeotuneActivityConfig>
    {
        [JsonProperty]
        [Option("STRINGS.UI.ACTIVITY.MULTIPLIER.TITLE",
            "STRINGS.UI.ACTIVITY.MULTIPLIER.TOOLTIP", Format = "F0")]
        [Limit(0, 200)]
        public int ActivityMultiplier { get; set; } = 2;
    }
}
