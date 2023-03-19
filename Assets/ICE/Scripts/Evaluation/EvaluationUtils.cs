using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ICE.Evaluation
{
#pragma warning disable CS0162 // Unreachable code detected
    public static class Utils
    {
        public static string GetAssetPath(Object asset)
        {
#if UNITY_EDITOR
            return AssetDatabase.GetAssetPath(asset);
#endif
            return "";
        }
#pragma warning restore CS0162 // Unreachable code detected

        public static string GetBlockType(GameObject block)
        {
            try
            {
                int sIndex = block.name.IndexOf("(Clone)", 0);
                return block.name.Substring(0, sIndex);
            }
            catch { }
            return block.name;
        }
    }
}