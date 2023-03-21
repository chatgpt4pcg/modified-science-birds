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

        [MenuItem("ICE/Evaluation/Stability Test", priority = 2)]
        public static void PlayStabilityEvaluatorScene()
        {
            ScenePlayerRestorable.Play("Assets/ICE/Scenes/StabilityTest.unity");
        }

        [MenuItem("ICE/Evaluation/Similarity Test", priority = 3)]
        public static void PlaySimilarityEvaluatorScene()
        {
            ScenePlayerRestorable.Play("Assets/ICE/Scenes/SimilarityTest.unity");
        }

        [MenuItem("ICE/Evaluation/Batched/Stability Test", priority = 4)]
        public static void PlayBatchedStabilityEvaluatorScene()
        {
            ScenePlayerRestorable.Play("Assets/ICE/Scenes/BatchStabilityTest.unity");
        }

        [MenuItem("ICE/Evaluation/Batched/Similarity Test", priority = 5)]
        public static void PlayBatchedSimilarityEvaluatorScene()
        {
            ScenePlayerRestorable.Play("Assets/ICE/Scenes/BatchSimilarityTest.unity");
        }
    }
}