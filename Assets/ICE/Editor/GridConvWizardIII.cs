using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ICE.Editor
{
    public class GridConvWizardIII : ScriptableWizard
    {
        [SerializeField] [TextArea] private string m_input;

        [MenuItem("ICE/Grid Level Converter MkIII")]
        private static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<GridConvWizardIII>("Create AB Level", "Create");
        }

        private void OnWizardCreate()
        {
            ABLevel lv = Grid2LevelConverterIII.ToLevel(m_input);
            string targetFile = EditorUtility.SaveFilePanel("Lv Converter", "", "MyLevel", "xml");
            if (!string.IsNullOrEmpty(targetFile))
            {
                LevelLoader.SaveXmlLevel(lv, targetFile);
            }
        }
    }
}