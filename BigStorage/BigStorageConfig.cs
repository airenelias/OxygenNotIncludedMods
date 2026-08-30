using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace BigStorage;

[ConfigFile(IndentOutput: true, SharedConfigLocation: true)]
[RestartRequired]
public class BigStorageConfig : SingletonOptions<BigStorageConfig>
{
    [JsonProperty]
    [Option("STRINGS.UI.CAPACITY.BIGSTORAGELOCKER.TITLE",
        "STRINGS.UI.CAPACITY.BIGSTORAGELOCKER.TOOLTIP", Format = "F0")]
    [Limit(2000, 2000000)]
    public int BigStorageLockerCapacity { get; set; } = 80000;

    [JsonProperty]
    [Option("STRINGS.UI.CAPACITY.BIGBEAUTIFULSTORAGELOCKER.TITLE",
        "STRINGS.UI.CAPACITY.BIGBEAUTIFULSTORAGELOCKER.TOOLTIP", Format = "F0")]
    [Limit(2000, 2000000)]
    public int BigBeautifulStorageLockerCapacity { get; set; } = 80000;

    [JsonProperty]
    [Option("STRINGS.UI.CAPACITY.BIGSMARTSTORAGELOCKER.TITLE",
        "STRINGS.UI.CAPACITY.BIGSMARTSTORAGELOCKER.TOOLTIP", Format = "F0")]
    [Limit(2000, 2000000)]
    public int BigSmartStorageLockerCapacity { get; set; } = 80000;

    [JsonProperty]
    [Option("STRINGS.UI.CAPACITY.BIGLIQUIDSTORAGE.TITLE",
        "STRINGS.UI.CAPACITY.BIGLIQUIDSTORAGE.TOOLTIP", Format = "F0")]
    [Limit(500, 500000)]
    public int BigLiquidStorageCapacity { get; set; } = 20000;

    [JsonProperty]
    [Option("STRINGS.UI.CAPACITY.BIGGASSTORAGE.TITLE",
        "STRINGS.UI.CAPACITY.BIGGASSTORAGE.TOOLTIP", Format = "F0")]
    [Limit(100, 100000)]
    public int BigGasStorageCapacity { get; set; } = 4000;

    [JsonProperty]
    [Option("STRINGS.UI.CAPACITY.BIGSTORAGETILE.TITLE",
        "STRINGS.UI.CAPACITY.BIGSTORAGETILE.TOOLTIP", Format = "F0")]
    [Limit(100, 100000)]
    public int BigStorageTileCapacity { get; set; } = 4000;

    [JsonProperty]
    [Option("STRINGS.UI.ENABLED.BIGREFRIGERATOR.TITLE",
        "STRINGS.UI.ENABLED.BIGREFRIGERATOR.TOOLTIP")]
    public bool BigRefrigeratorEnabled { get; set; } = true;

    [JsonProperty]
    [Option("STRINGS.UI.CAPACITY.BIGREFRIGERATOR.TITLE",
        "STRINGS.UI.CAPACITY.BIGREFRIGERATOR.TOOLTIP", Format = "F0")]
    [Limit(10, 10000)]
    public int BigRefrigeratorCapacity { get; set; } = 400;
}
