using HarmonyLib;
using UnityEngine;

namespace BigStorage;

    public class StorageBasedTint : KMonoBehaviour, ISim1000ms
    {
        public void Sim1000ms(float dt)
        {
            Reservoir component1 = this.gameObject.GetComponent<Reservoir>();
            if ((Object)component1 == (Object)null)
                return;

            Storage storage = Traverse.Create((object)component1).Field("storage").GetValue<Storage>();
            if ((Object)storage == (Object)null)
                return;
            float massStored = storage.MassStored();

            KBatchedAnimController kBatchedAnimController = this.gameObject.GetComponent<KBatchedAnimController>();
            if ((Object)kBatchedAnimController == (Object)null)
                return;

            Color colour = new Color(0f, 0f, 0f, 1f);
            Debug.Log("[BigStorage] color " + colour.ToString());

            foreach (GameObject gameObject in storage.items)
            {
                PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
                if ((Object)component != (Object)null)
                {
                    colour.r += ElementLoader.FindElementByHash(component.ElementID).substance.colour.r / 255f * (component.Mass / massStored);
                    colour.g += ElementLoader.FindElementByHash(component.ElementID).substance.colour.g / 255f * (component.Mass / massStored);
                    colour.b += ElementLoader.FindElementByHash(component.ElementID).substance.colour.b / 255f * (component.Mass / massStored);
                }
            }

            kBatchedAnimController.SetSymbolTint((KAnimHashedString)"gas_cloud", colour);
        }
    }
