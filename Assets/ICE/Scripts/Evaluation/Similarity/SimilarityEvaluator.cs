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
    public class SimilarityEvaluator : BaseSimilarityEvaluator
    {
        public Object inputDirectory;
        public Object outputDirectory;

        protected override IEnumerator IE_RunTest()
        {
            string inputDir = Utils.GetAssetPath(inputDirectory);
            string outputDir = Utils.GetAssetPath(outputDirectory);

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            Debug.Log("==============running similarity test==============");
            List<string> inputFiles = new List<string>(Directory.GetFiles(inputDir, "*.xml"));
            List<string> outputFiles = new List<string>();
            foreach (string i in inputFiles)
            {
                string tFileName = string.Format("{0}.png", Path.GetFileNameWithoutExtension(i));
                outputFiles.Add(Path.Combine(outputDir, tFileName));
            }

            yield return StartCoroutine(IE_TakeScreenshots(inputFiles, outputFiles));
            yield return null;
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Debug.Log("==============similarity test is done==============");
        }
    }
}