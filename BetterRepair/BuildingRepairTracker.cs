using KSerialization;

namespace BetterRepair
{
    [SerializationConfig(MemberSerialization.OptIn)]
    internal class BuildingRepairTracker : KMonoBehaviour, ISaveLoadable
    {
        [Serialize]
        public float damageTime = 0;

        [Serialize]
        public int repairAmount = 1;

        public void SetDamageTime()
        {
            damageTime = GameClock.Instance.GetTime();
        }

        public float GetDamageTimeThreshold()
        {
            return damageTime + BetterRepairTools.TimeThreshold * 600f;
        }

        public void SetRepairAmount(int repairAmount)
        {
            this.repairAmount = repairAmount;
        }

        public void UnsetRepairAmount()
        {
            this.repairAmount = 1;
        }
    }
}
