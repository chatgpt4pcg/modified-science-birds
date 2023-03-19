using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ICE.Editor
{
    [InitializeOnLoad]
    public class ScenePlayerRestorable
    {
        [Serializable]
        public struct SerializableSceneSetup
        {
            [SerializeField] private string m_path;
            [SerializeField] private bool m_isLoaded;
            [SerializeField] private bool m_isActive;
            [SerializeField] private bool m_isSubScene;

            public SerializableSceneSetup(SceneSetup sceneSetup)
            {
                m_path = sceneSetup.path;
                m_isLoaded = sceneSetup.isLoaded;
                m_isActive = sceneSetup.isActive;
                m_isSubScene = sceneSetup.isSubScene;
            }

            public SceneSetup GetSceneSetup()
            {
                return new SceneSetup() { path = m_path, isLoaded = m_isLoaded, isActive = m_isActive, isSubScene = m_isSubScene };
            }
        }

        [Serializable]
        public class SerializableSceneSetupList
        {
            [SerializeField] private List<SerializableSceneSetup> m_prevSceneSetups = new List<SerializableSceneSetup>();

            public SerializableSceneSetupList()
            { }

            public SerializableSceneSetupList(SceneSetup[] sceneSetups)
            {
                if (sceneSetups == null)
                {
                    throw new ArgumentNullException(nameof(sceneSetups));
                }
                foreach (SceneSetup sceneSetup in sceneSetups)
                {
                    m_prevSceneSetups.Add(new SerializableSceneSetup(sceneSetup));
                }
            }

            public SceneSetup[] GetSceneSetups()
            {
                SceneSetup[] sceneSetups = new SceneSetup[m_prevSceneSetups.Count];
                for (int i = 0; i < m_prevSceneSetups.Count; i++)
                {
                    sceneSetups[i] = m_prevSceneSetups[i].GetSceneSetup();
                }
                return sceneSetups;
            }
        }

        private const string PREV_SCENE_SETUP_KEY = nameof(ScenePlayerRestorable) + ".PrevSceneSetup";

        private static string PrevSceneSetupJson
        {
            get => EditorPrefs.GetString(PREV_SCENE_SETUP_KEY, "");
            set => EditorPrefs.SetString(PREV_SCENE_SETUP_KEY, value ?? "");
        }

        static ScenePlayerRestorable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        public static void Play(string scenePath)
        {
            if (!EditorApplication.isPlaying && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                PrevSceneSetupJson = EditorJsonUtility.ToJson(new SerializableSceneSetupList(EditorSceneManager.GetSceneManagerSetup()));
                EditorSceneManager.OpenScene(scenePath);
                EditorApplication.isPlaying = true;
            }
        }

        private static void OnPlayModeChanged(PlayModeStateChange pmsc)
        {
            if (pmsc == PlayModeStateChange.EnteredEditMode)
            {
                string prevSceneSetupJson = PrevSceneSetupJson;
                if (!string.IsNullOrEmpty(prevSceneSetupJson))
                {
                    SerializableSceneSetupList prevSceneSetupList = new SerializableSceneSetupList();
                    try
                    {
                        EditorJsonUtility.FromJsonOverwrite(prevSceneSetupJson, prevSceneSetupList);
                        EditorSceneManager.RestoreSceneManagerSetup(prevSceneSetupList.GetSceneSetups());
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    PrevSceneSetupJson = "";
                }
            }
        }
    }
}
