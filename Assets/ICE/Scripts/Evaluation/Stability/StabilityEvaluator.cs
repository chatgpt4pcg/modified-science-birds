using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ICE.Evaluation
{
    public class StabilityEvaluator : MonoBehaviour
    {
        public bool autoStart = true;
        public Object scene;
        public Object directory;
        public string outputName;
        public float duration;
        public float timeScale;
        public float movementThreshold = 3;
        public int retry;

        private bool m_isStable = false;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            if (autoStart)
                StartCoroutine(IE_Evaluate());
        }

        public IEnumerator IE_Evaluate()
        {
            Time.timeScale = timeScale;
            string dirPath = Utils.GetAssetPath(directory);
            string[] filepaths = Directory.GetFiles(dirPath, "*.xml");
            string[] xlevels = new string[filepaths.Length];
            for (int i = 0; i < xlevels.Length; i++)
                xlevels[i] = File.ReadAllText(filepaths[i]);
            LevelList.Instance.LoadLevelsFromSource(xlevels);

            StabilityData data = new StabilityData();

            List<int> unstables = new List<int>();
            for (int i = 0; i < xlevels.Length; i++)
            {
                Debug.LogFormat("--checking: {0}", filepaths[i]);
                yield return StartCoroutine(IE_StableCheck(i));
                data.raws.Add(new Stability() { tag = filepaths[i], isStable = m_isStable });
                if (!m_isStable)
                    unstables.Add(i);
            }

            //re-check
            int t = 0;
            while (unstables.Count > 0 && t < retry)
            {
                //shuffle unstable id
                for (int i = 0; i < unstables.Count - 1; i++)
                {
                    for (int j = i + 1; j < unstables.Count; j++)
                    {
                        if (Random.Range(0, 10) < 5)
                        {
                            int temp = unstables[i];
                            unstables[i] = unstables[j];
                            unstables[j] = temp;
                        }
                    }
                }
                //shuffle unstable id

                for (int i = unstables.Count - 1; i >= 0; i--)
                {
                    Debug.LogFormat("--re-checking: {0}", filepaths[i]);
                    yield return StartCoroutine(IE_StableCheck(unstables[i]));
                    data.raws[unstables[i]].isStable = m_isStable;
                    if (m_isStable)
                        unstables.RemoveAt(i);
                }
                t++;
            }
            //re-check

            data.dataCount = data.raws.Count;
            data.rate = (float)data.raws.FindAll(x=>x.isStable).Count / (float)data.dataCount;

            string jstring = JsonUtility.ToJson(data, true);
            File.WriteAllText(string.Format("{0}/{1}", dirPath, outputName), jstring);
            Time.timeScale = 1.0f;

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Debug.Log("============== stability check is done ==============");
        }

        IEnumerator IE_StableCheck(int currentIndex)
        {
            m_isStable = false;
            WaitForEndOfFrame wff = new WaitForEndOfFrame();

            string sceneName = Utils.GetAssetPath(scene);
            
            //load level
            yield return wff;
            LevelList.Instance.CurrentIndex = currentIndex;
            yield return wff;
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return wff;
            //load level

            int sObjCount = GetTotalObjectAmt();
            Dictionary<int, Vector2> sPos = GetObjectPositions();

            yield return wff;
            yield return new WaitForSeconds(duration);
            yield return wff;

            int eObjCount = GetTotalObjectAmt();
            Dictionary<int, Vector2> ePos = GetObjectPositions();

            m_isStable = sObjCount == eObjCount && !AnyMovingObject(sPos, ePos);
            yield return SceneManager.UnloadSceneAsync(sceneName);
            yield return wff;
        }

        private Dictionary<int, Vector2> GetObjectPositions()
        {
            ABBlock[] blocks = FindObjectsOfType<ABBlock>();
            ABPig[] pigs = FindObjectsOfType<ABPig>();
            ABTNT[] tnts = FindObjectsOfType<ABTNT>();

            Dictionary<int, Vector2> val = new Dictionary<int, Vector2>();
            foreach (ABBlock b in blocks)
            {
                try
                {
                    val.Add(b.gameObject.GetInstanceID(), b.transform.position);
                }
                catch (System.Exception error)
                {
                    Debug.LogError(error.ToString());
                }
            }
            foreach (ABPig p in pigs)
            {
                try
                {
                    val.Add(p.gameObject.GetInstanceID(), p.transform.position);
                }
                catch (System.Exception error)
                {
                    Debug.LogError(error.ToString());
                }
            }
            foreach (ABTNT t in tnts)
            {
                try
                {
                    val.Add(t.gameObject.GetInstanceID(), t.transform.position);
                }
                catch (System.Exception error)
                {
                    Debug.LogError(error.ToString());
                }
            }
            return val;
        }

        private bool AnyMovingObject(Dictionary<int, Vector2> startPositions, Dictionary<int, Vector2> endPositions)
        {
            if (startPositions.Count != endPositions.Count) //there is destroyed object(s)
            {
                return true;
            }

            foreach (int iId in endPositions.Keys)
            {
                if (!startPositions.ContainsKey(iId))
                {
                    continue;
                }
                float dist = Vector2.Distance(startPositions[iId], endPositions[iId]);
                if (dist >= movementThreshold)
                {
                    return true;
                }
            }
            return false;
        }

        private int GetTotalObjectAmt()
        {
            ABBlock[] blocks = FindObjectsOfType<ABBlock>();
            ABPig[] pigs = FindObjectsOfType<ABPig>();
            ABTNT[] tnts = FindObjectsOfType<ABTNT>();

            int val = blocks != null ? blocks.Length : 0;
            val += pigs != null ? pigs.Length : 0;
            val += tnts != null ? tnts.Length : 0;
            return val;
        }
    }
}