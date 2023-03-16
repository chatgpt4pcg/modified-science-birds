using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ICE.Editor
{
    public class ConverterWindow : ScriptableWizard
    {
        [SerializeField][TextArea] private string m_input;

        //[MenuItem("ICE/Level Converter")]
        private static void CreateWizard()
        {
            ScriptableWizard.DisplayWizard<ConverterWindow>("Create AB Level", "Create");
        }

        private void OnWizardCreate()
        {
            ABLevel lv = LevelConverter.ToLevel(m_input);
            string targetFile = EditorUtility.SaveFilePanel("Lv Converter", "", "MyLevel", "xml");
            if (!string.IsNullOrEmpty(targetFile))
            {
                LevelLoader.SaveXmlLevel(lv, targetFile);
            }
        }
    }
}