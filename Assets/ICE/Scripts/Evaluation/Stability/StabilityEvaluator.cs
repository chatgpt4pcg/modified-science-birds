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
    public class StabilityEvaluator : BaseStabilityEvaluator
    {
        public Object directory;
        public string outputName;

        protected override IEnumerator IE_RunTest()
        {
            Time.timeScale = timeScale;

            Debug.Log("==============running stability test==============");
            string dirPath = Utils.GetAssetPath(directory);
            List<string> inputFiles = new List<string>(Directory.GetFiles(dirPath, "*.xml"));
            string outputFile = string.Format("{0}/{1}", dirPath, outputName);

            yield return StartCoroutine(IE_Evaluate(inputFiles, outputFile));
            yield return null;
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Debug.Log("==============stability test is done==============");
        }
    }
}