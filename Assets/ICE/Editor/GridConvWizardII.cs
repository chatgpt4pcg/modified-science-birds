using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ICE.Editor
{
    public class GridConvWizardII : ScriptableWizard
    {
        [SerializeField] [TextArea] private string m_input;

        //[MenuItem("ICE/Grid Level Converter MkII")]
        private static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<GridConvWizardII>("Create AB Level", "Create");
        }

        private void OnWizardCreate()
        {
            ABLevel lv = Grid2LevelConverterII.ToLevel(m_input);
            string targetFile = EditorUtility.SaveFilePanel("Lv Converter", "", "MyLevel", "xml");
            if (!string.IsNullOrEmpty(targetFile))
            {
                LevelLoader.SaveXmlLevel(lv, targetFile);
            }
        }
    }
}