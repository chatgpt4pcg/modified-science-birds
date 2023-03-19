using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ICE.Editor
{
    public class GridConverterWizard : ScriptableWizard
    {
        [SerializeField] [TextArea(25, 25)] private string m_input;

        private void OnWizardCreate()
        {
            string targetFile = EditorUtility.SaveFilePanel("Lv Converter", "", "MyLevel", "xml");
            if (!string.IsNullOrEmpty(targetFile))
            {
                try
                {
                    GridLevelConverter converter = Resources.Load<GridLevelConverter>(nameof(GridLevelConverter));
                    ABLevel lv = converter.ToLevel(m_input);
                    LevelLoader.SaveXmlLevel(lv, targetFile);
                }
                catch (System.Exception error)
                {
                    Debug.LogError(error.ToString());
                }
            }
        }
    }
}