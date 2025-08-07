namespace BetterRepair
{
    internal class STRINGS
    {
        public static class CHORES
        {
            public static class PRECONDITIONS
            {
                public static LocString ABOVE_THRESHOLD = "Above repair threshold";
            }
        }

        public static class UI
        {
            public static class THRESHOLD
            {
                public static class CONDITION
                {
                    public static LocString TITLE = "Repair threshold (%)";
                    public static LocString TOOLTIP = "Repair errand becomes doable only if building condition drops to this level";
                }

                public static class TIME
                {
                    public static LocString TITLE = "Complete Repair threshold (cycles)";
                    public static LocString TOOLTIP = "Full building repair will be erranded only if it didn't recieve any damage for this amount of cycles";
                }
            }

            public static class MULTIPLIER
            {
                public static LocString TITLE = "Repair Speed";

                public static class OVERALL
                {
                    public static LocString TITLE = "Overall Repair speed (%)";
                    public static LocString TOOLTIP = "How quickly Repair will be finished";
                }

                public static class CONSTRUCTION
                {
                    public static LocString TITLE = "Construction Repair speed bonus (%)";
                    public static LocString TOOLTIP = "Construction (Building) impact on Repair effectiveness";
                }

                public static class MACHINERY
                {
                    public static LocString TITLE = "Machinery Repair speed bonus (%)";
                    public static LocString TOOLTIP = "Machinery (Operating) impact on Repair effectiveness";
                }

                public static class STRENGTH
                {
                    public static LocString TITLE = "Strength Repair speed bonus (%)";
                    public static LocString TOOLTIP = "Strength impact on Repair effectiveness";
                }
            }

            public static class RESTORE_TEMPERATURE
            {
                public static LocString TITLE = "Repair calms temperature";
                public static LocString TOOLTIP = "Repair gradually restores buildings default temperature";
            }

            public static class CHORE
            {
                public static LocString TITLE = "Repair Errand Type";

                public static class TIDYING
                {
                    public static LocString TITLE = "Tidying";
                    public static LocString TOOLTIP = "Duplicants with set Tidying errand can do Repair";
                }

                public static class BUILDING
                {
                    public static LocString TITLE = "Building";
                    public static LocString TOOLTIP = "Duplicants with set Building errand can do Repair";
                }

                public static class OPERATING
                {
                    public static LocString TITLE = "Operating";
                    public static LocString TOOLTIP = "Duplicants with set Operating errand can do Repair";
                }
            }
        }
    }
}
