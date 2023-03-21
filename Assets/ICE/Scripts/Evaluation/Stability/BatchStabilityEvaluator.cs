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
    public class BatchStabilityEvaluator : BaseStabilityEvaluator
    {
        [SerializeField] private string m_directory;

        protected override IEnumerator IE_RunTest()
        {
            Time.timeScale = timeScale;

            Debug.Log("==============running stability test (batch)==============");
            Dictionary<string, List<string>> xFiles = Utils.GetXMLFileBatch(m_directory);
            foreach (string k in xFiles.Keys)
            {
                string outputFile = string.Format("{0}.json", k.Replace("levels", "stability"));
                string outDir = Path.GetDirectoryName(outputFile);
                if (!Directory.Exists(outDir))
                {
                    Directory.CreateDirectory(outDir);
                }

                List<string> inputFiles = xFiles[k];
                yield return StartCoroutine(IE_Evaluate(inputFiles, outputFile));
                yield return null;
            }
            yield return null;
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Debug.Log("==============stability test (batch) is done==============");
        }
    }
}