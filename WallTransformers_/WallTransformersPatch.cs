using ClipperLib;
using HarmonyLib;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WallTransformers
{
    [HarmonyPatch]
    public class WallTransformersPatch
    {
        public static PermittedRotations TRANSFORMER_PERMITTED_ROTATIONS = PermittedRotations.R360;

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Rotatable), "GetVisualizerRotation")]
        public static float GetVisualizerRotation(object instance)
        {
            // its a stub so it has no initial content
            throw new NotImplementedException("It's a stub");
        }

        [HarmonyReversePatch]
        [HarmonyPatch(typeof(Rotatable), "OrientVisualizer")]
        public static void OrientVisualizer(object instance, Orientation orientation)
        {
            // its a stub so it has no initial content
            throw new NotImplementedException("It's a stub");
        }

        /*[HarmonyReversePatch]
        [HarmonyPatch(typeof(Rotatable), "GetComponent")]
        public static T GetComponent<T>(object instance)
        {
            // its a stub so it has no initial content
            throw new NotImplementedException("It's a stub");
        }*/

        [HarmonyPatch(typeof(Rotatable), "Rotate")]
        public class PowerTransformerRotatablePatch
        {
            private static bool rotated = false;
            private static bool flipped = false;

            public static void Prefix(Rotatable __instance, ref Vector3 ___pivot, ref Vector3 ___visualizerOffset,
                ref int ___width, ref int ___height, ref Orientation ___orientation, ref PermittedRotations ___permittedRotations)
            {
                KBatchedAnimController kBatchedAnimController = __instance.GetComponent<KBatchedAnimController>();
                KBoxCollider2D kBoxCollider2D = __instance.GetComponent<KBoxCollider2D>();
                Console.WriteLine("==========> WallTransformers : PRE : "
                    + ___orientation.ToString() + " " + ___permittedRotations.ToString() + (rotated ? " rotated" : "") + (flipped ? " flipped" : ""));
                if (kBatchedAnimController != null) Console.WriteLine("==========> WallTransformers : ANIM : "
                    + kBatchedAnimController.Pivot.ToString() + " "
                    + kBatchedAnimController.Rotation.ToString() + " "
                    + kBatchedAnimController.Offset.ToString() + " "
                    + kBatchedAnimController.FlipX.ToString() + " "
                    + kBatchedAnimController.FlipY.ToString());
                if (kBoxCollider2D != null) Console.WriteLine("==========> WallTransformers : COLLIDER : "
                    + kBoxCollider2D.offset.ToString() + " "
                    + kBoxCollider2D.size.ToString());

                if (___orientation == Orientation.R270)
                {
                    rotated = true;
                }
                if (rotated && ___orientation == Orientation.Neutral && !flipped)
                {
                    ___permittedRotations = PermittedRotations.FlipH;
                    flipped = true;
                    rotated = false;
                }
                else if (___orientation == Orientation.FlipH)
                {
                    ___pivot = new Vector3(0.5f, 0.5f, 0);
                    ___visualizerOffset = new Vector3(-0.5f, 0, 0);
                    kBatchedAnimController.Pivot = new Vector3(0.5f, 0.5f, 0);
                    kBatchedAnimController.Offset = new Vector3(-0.5f, 0, 0);
                    kBatchedAnimController.FlipX = false;
                    ___orientation = Orientation.Neutral;
                    ___permittedRotations = PermittedRotations.R360;
                }
                else if (rotated && ___orientation == Orientation.Neutral && flipped)
                {
                    ___pivot = new Vector3(-0.5f, 0.5f, 0);
                    ___visualizerOffset = new Vector3(0.5f, 0, 0);
                    kBatchedAnimController.Pivot = new Vector3(-0.5f, 0.5f, 0);
                    kBatchedAnimController.Offset = new Vector3(0.5f, 0, 0);
                    kBatchedAnimController.FlipX = false;
                    ___permittedRotations = PermittedRotations.FlipH;
                    flipped = false;
                    rotated = false;
                }
                Console.WriteLine("==========> WallTransformers : CHANGE : "
                    + ___orientation.ToString() + " " + ___permittedRotations.ToString() + (rotated ? " rotated" : "") + (flipped ? " flipped" : ""));
                if (kBatchedAnimController != null) Console.WriteLine("==========> WallTransformers : ANIM : "
                    + kBatchedAnimController.Pivot.ToString() + " "
                    + kBatchedAnimController.Rotation.ToString() + " "
                    + kBatchedAnimController.Offset.ToString() + " "
                    + kBatchedAnimController.FlipX.ToString() + " "
                    + kBatchedAnimController.FlipY.ToString());
                if (kBoxCollider2D != null) Console.WriteLine("==========> WallTransformers : COLLIDER : "
                    + kBoxCollider2D.offset.ToString() + " "
                    + kBoxCollider2D.size.ToString());
            }

            public static void Postfix(Rotatable __instance, ref Vector3 ___pivot, ref Vector3 ___visualizerOffset,
                ref int ___width, ref int ___height, ref Orientation ___orientation, ref PermittedRotations ___permittedRotations)
            {
                KBatchedAnimController kBatchedAnimController = __instance.GetComponent<KBatchedAnimController>();
                KBoxCollider2D kBoxCollider2D = __instance.GetComponent<KBoxCollider2D>();
                Console.WriteLine("==========> WallTransformers : POST : "
                    + ___orientation.ToString() + " " + ___permittedRotations.ToString() + (rotated ? " rotated" : "") + (flipped ? " flipped" : ""));
                if (kBatchedAnimController != null) Console.WriteLine("==========> WallTransformers : ANIM : "
                    + kBatchedAnimController.Pivot.ToString() + " "
                    + kBatchedAnimController.Rotation.ToString() + " "
                    + kBatchedAnimController.Offset.ToString() + " "
                    + kBatchedAnimController.FlipX.ToString() + " "
                    + kBatchedAnimController.FlipY.ToString());
                if (kBoxCollider2D != null) Console.WriteLine("==========> WallTransformers : COLLIDER : "
                    + kBoxCollider2D.offset.ToString() + " "
                    + kBoxCollider2D.size.ToString());
            }
        }

        [HarmonyPatch(typeof(BuildingConfigManager), "OnPrefabInit")]
        internal class ApplyBuildingButton
        {
            public static void Postfix(ref GameObject ___baseTemplate)
            {
                System.Console.WriteLine("WallTransformers: " + ___baseTemplate.GetType());

                ___baseTemplate.AddComponent<RotateTypeButton>();
            }
        }

        [HarmonyPatch(typeof(PowerTransformerConfig), "ConfigureBuildingTemplate")]
        public class PowerTransformerPatch
        {
            public static void Postfix(GameObject go, Tag prefab_tag)
            {
                go.GetComponent<Building>().Def.BuildLocationRule = BuildLocationRule.Anywhere;
                go.GetComponent<Building>().Def.PermittedRotations = TRANSFORMER_PERMITTED_ROTATIONS;
            }
        }

        [HarmonyPatch(typeof(PowerTransformerSmallConfig), "ConfigureBuildingTemplate")]
        public class PowerTransformerSmallPatch
        {
            public static void Postfix(GameObject go, Tag prefab_tag)
            {
                go.GetComponent<Building>().Def.BuildLocationRule = BuildLocationRule.Anywhere;
                go.GetComponent<Building>().Def.PermittedRotations = TRANSFORMER_PERMITTED_ROTATIONS;
            }
        }
    }
}
