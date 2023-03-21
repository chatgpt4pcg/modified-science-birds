using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        public static Dictionary<string, List<string>> GetXMLFileBatch(string directory)
        {
            Dictionary<string, List<string>> val = new Dictionary<string, List<string>>();
            List<string> xFiles = new List<string>(Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories));
            foreach (string f in xFiles)
            {
                string tKey = Path.GetDirectoryName(f);
                if (!val.ContainsKey(tKey))
                {
                    val.Add(tKey, new List<string>());
                }
                val[tKey].Add(f);
            }
            return val;
        }
    }
}