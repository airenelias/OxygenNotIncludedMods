using static STRINGS.UI;

namespace BigStorage;

public static class STRINGS
{
    public static class BUILDINGS
    {
        public static class PREFABS
        {
            public static class BIGSTORAGELOCKER
            {
                public static LocString NAME = FormatAsLink("Big Storage Bin", "BIGSTORAGELOCKER");
                public static LocString DESC = "We removed the unused tea kettle and added a moon roof!";
                public static LocString EFFECT = "Stores a greater amount of the " + FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " of your choosing.";
            }

            public static class BIGBEAUTIFULSTORAGELOCKER
            {
                public static LocString NAME = FormatAsLink("Big Beautiful Storage Bin", "BIGBEAUTIFULSTORAGELOCKER");
                public static LocString DESC = "Is it storage, or is it art?";
                public static LocString EFFECT = "Stores a greater amount of the " + FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " of your choosing.\n\nIncreases " + FormatAsLink("Decor", "DECOR") + ", contributing to " + FormatAsLink("Morale", "MORALE") + ".";
            }

            public static class BIGSMARTSTORAGELOCKER
            {
                public static LocString NAME = FormatAsLink("Big Smart Storage Bin", "BIGSMARTSTORAGELOCKER");
                public static LocString DESC = "Even more space for your smart solutions!";
                public static LocString EFFECT = "Stores a greater amount of the " + FormatAsLink("Solid Materials", "ELEMENTS_SOLID") + " of your choosing.\n\nSends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when bin is full.";
                public static LocString LOGIC_PORT = "Full/Not Full";
                public static LocString LOGIC_PORT_ACTIVE = "Sends a " + FormatAsAutomationState("Green Signal", AutomationState.Active) + " when full";
                public static LocString LOGIC_PORT_INACTIVE = "Otherwise, sends a " + FormatAsAutomationState("Red Signal", AutomationState.Standby);
            }

            public static class BIGLIQUIDSTORAGE
            {
                public static LocString NAME = FormatAsLink("Big Liquid Reservoir", "BIGLIQUIDSTORAGE");
                public static LocString DESC = "Compacted into buckyballs, maybe. Who knows?";
                public static LocString EFFECT = "Stores a greater amount of the " + FormatAsLink("Liquid", "ELEMENTS_LIQUID") + " resources piped into it.";
            }

            public static class BIGGASSTORAGE
            {
                public static LocString NAME = FormatAsLink("Big Gas Reservoir", "BIGGASSTORAGE");
                public static LocString DESC = "Ten times the space at twenty times the pressure!";
                public static LocString EFFECT = "Stores a greater amount of the " + FormatAsLink("Gas", "ELEMENTS_GAS") + " resources piped into it.";
            }
            public static class BIGGASRESERVOIR
            {
                public static LocString NAME = FormatAsLink("Big Gas Reservoir", "BIGGASRESERVOIR");
                public static LocString DESC = "Ten times the space at twenty times the pressure!";
                public static LocString EFFECT = "Stores a greater amount of the " + FormatAsLink("Gas", "ELEMENTS_GAS") + " resources piped into it.";
            }

            public static class BIGSTORAGETILE
            {
                public static LocString NAME = FormatAsLink("Big Storage Tile", "BIGSTORAGETILE");
                public static LocString DESC = "Will it store more if you keep jumping on it?";
                public static LocString EFFECT = "Used to build the walls and floors of rooms.\n\nProvides a greater amount of built-in storage for small spaces.";
            }

            public static class BIGREFRIGERATOR
            {
                public static LocString NAME = FormatAsLink("Big Refrigerator", "BIGREFRIGERATOR");
                public static LocString DESC = "With this much space, four duplicants can now fit in there!\nHowever, leaving duplicants inside for an extended amount of time is not advised.";
                public static LocString EFFECT = "Stores a greater amount of " + FormatAsLink("Food", "FOOD") + " at an ideal " + FormatAsLink("Temperature", "HEAT") + " to prevent spoilage.";
            }
        }
    }

    public static class UI
    {
        public static class CAPACITY
        {
            public static class BIGSTORAGELOCKER
            {
                public static LocString TITLE = "Big Storage Bin Capacity (kg)";
                public static LocString TOOLTIP = "Determines the capacity of the Big Storage Bin";
            }

            public static class BIGBEAUTIFULSTORAGELOCKER
            {
                public static LocString TITLE = "Big Beautiful Storage Bin Capacity (kg)";
                public static LocString TOOLTIP = "Determines the capacity of the Big Beautiful Storage Bin";
            }

            public static class BIGSMARTSTORAGELOCKER
            {
                public static LocString TITLE = "Big Smart Storage Bin Capacity (kg)";
                public static LocString TOOLTIP = "Determines the capacity of the Big Smart Storage Bin";
            }

            public static class BIGLIQUIDSTORAGE
            {
                public static LocString TITLE = "Big Liquid Reservoir Capacity (kg)";
                public static LocString TOOLTIP = "Determines the capacity of the Big Liquid Reservoir\nCAUTION: Reducing reservoir capacity during playthrough may result in the loss of all stored resources; ensure that you have stored your excesses before lowering this value";
            }

            public static class BIGGASSTORAGE
            {
                public static LocString TITLE = "Big Gas Reservoir Capacity (kg)";
                public static LocString TOOLTIP = "Determines the capacity of the Big Gas Reservoir\nCAUTION: Reducing reservoir capacity during playthrough may result in the loss of all stored resources; ensure that you have stored your excesses before lowering this value";
            }

            public static class BIGSTORAGETILE
            {
                public static LocString TITLE = "Big Storage Tile Capacity (kg)";
                public static LocString TOOLTIP = "Determines the capacity of the Big Storage Tile";
            }

            public static class BIGREFRIGERATOR
            {
                public static LocString TITLE = "Big Refrigerator Capacity (kg)";
                public static LocString TOOLTIP = "Determines the capacity of the Big Refrigerator";
            }
        }

        public static class ENABLED
        {
            public static class BIGREFRIGERATOR
            {
                public static LocString TITLE = "Enable Big Refrigerator";
                public static LocString TOOLTIP = "Allows research and construction of the Big Refrigerator";
            }
        }
    }
}
