using KSerialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SkilledEnough
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class SkilledEnoughSaveData : KMonoBehaviour, ISaveLoadable
    {
        [Serialize]
        public List<string> SkilledEnoughList = new List<string>();

        public static SkilledEnoughSaveData Instance { get; private set; }

        public SkilledEnoughSaveData()
        {
            Instance = this;
        }

        [OnSerializing]
        public void OnSerializing()
        {
            SaveData();
        }

        private void SaveData()
        {
            SkilledEnoughList.Clear();
            foreach (MinionResume minionResume in Components.MinionResumes)
            {
                if (minionResume.HasTag(SkilledEnoughTools.SkilledEnough))
                {
                    SkilledEnoughList.Add(minionResume.GetIdentity.nameStringKey);
                }
            }
        }

        [OnDeserializing]
        public void OnDeserializing()
        {
            SkilledEnoughList.Clear();
        }

        internal void LoadData()
        {
            // skipping if already loaded somewhere else
            if (SkilledEnoughList.Count == 0)
            {
                return;
            }

            foreach (MinionResume minionResume in Components.MinionResumes)
            {
                if (SkilledEnoughList.Contains(minionResume.GetIdentity.nameStringKey))
                {
                    minionResume.AddTag(SkilledEnoughTools.SkilledEnough);
                }
            }
            SkilledEnoughList.Clear();
        }
    }
}
