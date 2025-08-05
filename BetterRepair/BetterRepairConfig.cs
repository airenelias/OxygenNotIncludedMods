using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace BetterRepair
{
    [ConfigFile(IndentOutput: true, SharedConfigLocation: true)]
    internal class BetterRepairConfig : SingletonOptions<BetterRepairConfig>
    {
        [JsonProperty]
        [Option("STRINGS.UI.MULTIPLIER.OVERALL.TITLE",
            "STRINGS.UI.MULTIPLIER.OVERALL.TOOLTIP", Format = "F0")]
        [Limit(0, 1000)]
        public int OverallSpeedMultiplier { get; set; } = 100;

        [JsonProperty]
        [Option("STRINGS.UI.MULTIPLIER.CONSTRUCTION.TITLE",
            "STRINGS.UI.MULTIPLIER.CONSTRUCTION.TOOLTIP", Format = "F0")]
        [Limit(0, 100)]
        public int ConstructionSpeedMultiplier { get; set; } = 25;

        [JsonProperty]
        [Option("STRINGS.UI.MULTIPLIER.MACHINERY.TITLE",
            "STRINGS.UI.MULTIPLIER.MACHINERY.TOOLTIP", Format = "F0")]
        [Limit(0, 100)]
        public int MachinerySpeedMultiplier { get; set; } = 15;

        [JsonProperty]
        [Option("STRINGS.UI.MULTIPLIER.STRENGTH.TITLE",
            "STRINGS.UI.MULTIPLIER.STRENGTH.TOOLTIP", Format = "F0")]
        [Limit(0, 100)]
        public int StrengthSpeedMultiplier { get; set; } = 5;

        [JsonProperty]
        [Option("STRINGS.UI.CHORE.TIDYING.TITLE",
            "STRINGS.UI.CHORE.TIDYING.TOOLTIP",
            "STRINGS.UI.CHORE.TITLE")]
        public bool RepairIsTidyingChore { get; set; } = false;

        [JsonProperty]
        [Option("STRINGS.UI.CHORE.BUILDING.TITLE",
            "STRINGS.UI.CHORE.BUILDING.TOOLTIP",
            "STRINGS.UI.CHORE.TITLE")]
        public bool RepairIsBuildingChore { get; set; } = true;

        [JsonProperty]
        [Option("STRINGS.UI.CHORE.OPERATING.TITLE",
            "STRINGS.UI.CHORE.OPERATING.TOOLTIP",
            "STRINGS.UI.CHORE.TITLE")]
        public bool RepairIsOperatingChore { get; set; } = true;
    }
}
