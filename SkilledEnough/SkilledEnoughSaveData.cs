using KSerialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace SkilledEnough
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public class SkilledEnoughSaveData : KMonoBehaviour, ISaveLoadable
    {
        [Serialize]
        public string SaveVersion = null;

        [Serialize]
        public List<int> SkilledEnoughIdList = new List<int>();

        [Obsolete("Use InstanceID in SkilledEnoughIdList instead")]
        [Serialize]
        public List<string> SkilledEnoughList = new List<string>();

        private bool loaded = false;

        public static SkilledEnoughSaveData Instance { get; private set; }

        public SkilledEnoughSaveData()
        {
            Instance = this;
        }

        private bool IsLoaded()
        {
            return loaded;
        }

        private void SetLoaded()
        {
            loaded = true;
        }

        private void ResetSaveData(bool saving)
        {
            // support for older saves
            if (SaveVersion == null)
            {
#pragma warning disable CS0618
                SkilledEnoughList.Clear();
#pragma warning restore CS0618
            }

            SaveVersion = null;
            SkilledEnoughIdList.Clear();
            // flag to prevent the load from triggering if continue playing same save
            loaded = saving;
        }

        [OnSerializing]
        public void OnSerializing()
        {
            SaveData();
        }

        private void SaveData()
        {
            // reset before saving just in case
            ResetSaveData(true);
            SaveVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            foreach (MinionResume minionResume in Components.MinionResumes)
            {
                if (minionResume.HasTag(SkilledEnoughTools.SkilledEnough))
                {
                    KPrefabID prefabId = minionResume.GetComponent<KPrefabID>();
                    if (prefabId != null)
                        SkilledEnoughIdList.Add(prefabId.InstanceID);
                }
            }
        }

        [OnDeserializing]
        public void OnDeserializing()
        {
            // reset before loading in case if loading for second time
            ResetSaveData(false);
        }

        internal void LoadData()
        {
            // skipping load if already loaded somewhere else
            if (IsLoaded())
            {
                return;
            }

            // support for older saves
            if (SaveVersion == null)
            {
                foreach (MinionResume minionResume in Components.MinionResumes)
                {
#pragma warning disable CS0618
                    if (SkilledEnoughList.Contains(minionResume.GetIdentity.nameStringKey))
#pragma warning restore CS0618
                    {
                        KPrefabID prefabId = minionResume.GetComponent<KPrefabID>();
                        if (prefabId != null)
                            SkilledEnoughIdList.Add(minionResume.GetComponent<KPrefabID>().InstanceID);
                    }
                }
            }

            foreach (MinionResume minionResume in Components.MinionResumes)
            {
                KPrefabID prefabId = minionResume.GetComponent<KPrefabID>();
                if (prefabId == null)
                    continue;

                if (SkilledEnoughIdList.Contains(prefabId.InstanceID))
                {
                    minionResume.AddTag(SkilledEnoughTools.SkilledEnough);
                }
            }

            // flag after loading game to prevent the load from triggering again
            SetLoaded();
        }
    }
}
