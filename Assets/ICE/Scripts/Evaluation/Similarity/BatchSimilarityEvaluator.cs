using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ICE.Evaluation
{
    public class BatchSimilarityEvaluator : BaseSimilarityEvaluator
    {
        [SerializeField] private string m_directory;

        protected override IEnumerator IE_RunTest()
        {
            Debug.Log("==============running similarity test (batched)==============");
            Dictionary<string, List<string>> xFiles = Utils.GetXMLFileBatch(m_directory);
            foreach (string k in xFiles.Keys)
            {
                string outDir = k.Replace("levels", "images");
                if (!Directory.Exists(outDir))
                {
                    Directory.CreateDirectory(outDir);
                }

                List<string> inputFiles = xFiles[k];
                List<string> outputFiles = new List<string>();
                foreach (string i in inputFiles)
                {
                    string tFileName = string.Format("{0}.png", Path.GetFileNameWithoutExtension(i));
                    outputFiles.Add(Path.Combine(outDir, tFileName));
                }

                Debug.LogFormat("-working on: {0}", k);
                yield return new WaitForEndOfFrame();
                yield return StartCoroutine(IE_TakeScreenshots(inputFiles, outputFiles));
                yield return new WaitForEndOfFrame();
            }
            yield return null;
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Debug.Log("==============similarity test (batched) is done==============");
        }
    }
}