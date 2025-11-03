using System;
using UnityEngine;

namespace WallTransformers
{
    public class RotateTypeButton : KMonoBehaviour
    {
        protected override void OnPrefabInit()
        {
            Subscribe((int)GameHashes.RefreshUserMenu, (object data) => OnRefreshUserMenu());
        }

        private void OnRefreshUserMenu()
        {
            Action result;
            Enum.TryParse<Action>("NumActions", out result);
            Game.Instance.userMenu.AddButton(this.gameObject,
                new KIconButtonMenu.ButtonInfo("", Nummy().ToString(), new System.Action(OnClick)));
        }

        internal virtual void OnClick()
        {
            /*UnityEngine.Component[] components = gameObject.GetComponents(typeof(UnityEngine.Component));
            if (components != null)
                foreach (UnityEngine.Component comp in components)
                {
                    Console.WriteLine("WallTransformers : gameObject.GetComponents : " + comp.name + " " + comp.GetType() + " " + comp.ToString());
                }
            components = gameObject.GetComponentsInParent(typeof(UnityEngine.Component));
            if (components != null)
                foreach (UnityEngine.Component comp in components)
                {
                    Console.WriteLine("WallTransformers : gameObject.GetComponentsInParent : " + comp.name + " " + comp.GetType() + " " + comp.ToString());
                }
            components = gameObject.GetComponentsInChildren(typeof(UnityEngine.Component));
            if (components != null)
                foreach (UnityEngine.Component comp in components)
                {
                    Console.WriteLine("WallTransformers : gameObject.GetComponentsInChildren : " + comp.name + " " + comp.GetType() + " " + comp.ToString());
                }*/

            if (WallTransformersPatch.TRANSFORMER_PERMITTED_ROTATIONS == PermittedRotations.R360)
                WallTransformersPatch.TRANSFORMER_PERMITTED_ROTATIONS = PermittedRotations.FlipH;
            else
                WallTransformersPatch.TRANSFORMER_PERMITTED_ROTATIONS = PermittedRotations.R360;
            Game.Instance.userMenu.Refresh(base.gameObject);
            Nummy();
            Nummytemp();
        }

        private int Nummy()
        {
            int a = 3;
            int b = 2;
            return a + b;
        }

        private int Nummytemp()
        {
            int a = 3;
            int b = 2;
            return a - b;
        }
    }
}
