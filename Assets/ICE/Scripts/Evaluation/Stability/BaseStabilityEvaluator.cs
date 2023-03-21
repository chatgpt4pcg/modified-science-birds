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
    public class BaseStabilityEvaluator : MonoBehaviour
    {
        public bool autoStart = true;
        public Object scene;
        public float duration;
        public float timeScale;
        public float movementThreshold = 3;
        public int trials;

        private float m_score = 0;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        protected virtual void Start()
        {
            if (autoStart)
                StartCoroutine(IE_RunTest());
        }

        protected virtual IEnumerator IE_RunTest()
        {
            yield return null;
        }

        protected IEnumerator IE_Evaluate(List<string> inputFiles, string outputFile)
        {
            timeScale = Mathf.Max(0.1f, timeScale);
            Time.timeScale = timeScale;

            string[] xlevels = new string[inputFiles.Count];
            for (int i = 0; i < xlevels.Length; i++)
                xlevels[i] = File.ReadAllText(inputFiles[i]);
            LevelList.Instance.LoadLevelsFromSource(xlevels);

            StabilityData data = new StabilityData();
            trials = Mathf.Max(1, trials);
            for (int i = 0; i < xlevels.Length; i++)
            {
                float score = 0;
                for (int n = 0; n < trials; n++)
                {
                    Debug.LogFormat("--({0})checking: {1}", n, inputFiles[i]);
                    yield return StartCoroutine(IE_StableCheck(i));
                    score = Mathf.Max(score, m_score);
                    if (score >= 1.0f) //already a max score
                    {
                        break;
                    }
                }
                data.raws.Add(new Stability() { tag = inputFiles[i], score = score });
            }

            data.dataCount = data.raws.Count;

            float rate = 0;
            foreach (Stability d in data.raws)
            {
                rate += d.score;
            }

            data.rate = rate / (float)data.dataCount;

            string jstring = JsonUtility.ToJson(data, true);
            File.WriteAllText(outputFile, jstring);
            Debug.LogFormat("--data saved to: {0}", outputFile);

            Time.timeScale = 1.0f;
            yield return new WaitForEndOfFrame();
        }

        IEnumerator IE_StableCheck(int currentIndex)
        {
            m_score = 0;
            WaitForEndOfFrame wff = new WaitForEndOfFrame();

            string sceneName = Utils.GetAssetPath(scene);

            //load level
            yield return wff;
            LevelList.Instance.CurrentIndex = currentIndex;
            yield return wff;
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return wff;
            //load level

            float sObjCount = GetTotalObjectAmt();
            yield return wff;
            yield return new WaitForSeconds(duration);
            yield return wff;
            float eObjCount = GetTotalObjectAmt();

            //m_isStable = sObjCount == eObjCount && !AnyMovingObject(sPos, ePos);

            try
            {
                m_score = eObjCount / sObjCount;
            }
            catch 
            {
                m_score = 0;
            }

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