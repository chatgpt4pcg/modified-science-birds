using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ICE.Editor
{
    public class GridConvWizard : ScriptableWizard
    {
        [SerializeField] [TextArea] private string m_input;

        //[MenuItem("ICE/Grid Level Converter")]
        private static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<GridConvWizard>("Create AB Level", "Create");
        }

        private void OnWizardCreate()
        {
            ABLevel lv = Grid2LevelConverter.ToLevel(m_input);
            string targetFile = EditorUtility.SaveFilePanel("Lv Converter", "", "MyLevel", "xml");
            if (!string.IsNullOrEmpty(targetFile))
            {
                LevelLoader.SaveXmlLevel(lv, targetFile);
            }
        }
    }
}