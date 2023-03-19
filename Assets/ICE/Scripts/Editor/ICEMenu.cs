using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace ICE.Editor
{
    public static class ICEMenu
    {
        [MenuItem("ICE/Grid Level Converter", priority = 1)]
        private static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<GridConverterWizard>("Create AB Level", "Create");
        }

        [MenuItem("ICE/Evaluation/Stability", priority = 2)]
        public static void PlayStabilityEvaluatorScene()
        {
            ScenePlayerRestorable.Play("Assets/ICE/Scenes/StabilityTest.unity");
        }

        [MenuItem("ICE/Evaluation/Similarity", priority = 3)]
        public static void PlaySimilarityEvaluatorScene()
        {
            ScenePlayerRestorable.Play("Assets/ICE/Scenes/SimilarityTest.unity");
        }
    }
}