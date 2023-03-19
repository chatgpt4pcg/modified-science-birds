using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ICE.Editor
{
    [CustomEditor(typeof(BaseLevelConverter), true)]
    [CanEditMultipleObjects]
    public class EditorLevelConverter : UnityEditor.Editor
    {
        private string m_gptString;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            m_gptString = EditorGUILayout.TextArea(m_gptString, GUILayout.Height(250));
            if (GUILayout.Button("Convert"))
            {
                Convert(m_gptString);
            }
        }

        public void Convert(string gptString)
        {
            BaseLevelConverter converter = serializedObject.targetObject as BaseLevelConverter;
            string targetFile = EditorUtility.SaveFilePanel("Lv Converter", "", "MyLevel", "xml");
            if (!string.IsNullOrEmpty(targetFile))
            {
                ABLevel lv = converter.ToLevel(gptString);
                LevelLoader.SaveXmlLevel(lv, targetFile);
            }
        }
    }
}