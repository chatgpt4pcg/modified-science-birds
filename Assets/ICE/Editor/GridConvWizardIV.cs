using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ICE.Editor
{
    public class GridConvWizardIV : ScriptableWizard
    {
        [SerializeField] [TextArea] private string m_input;

        //[MenuItem("ICE/Grid Level Converter MkIV")]
        private static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<GridConvWizardIV>("Create AB Level", "Create");
        }

        private void OnWizardCreate()
        {
            ABLevel lv = Grid2LevelConverterIV.ToLevel(m_input);
            string targetFile = EditorUtility.SaveFilePanel("Lv Converter", "", "MyLevel", "xml");
            if (!string.IsNullOrEmpty(targetFile))
            {
                LevelLoader.SaveXmlLevel(lv, targetFile);
            }
        }
    }
}