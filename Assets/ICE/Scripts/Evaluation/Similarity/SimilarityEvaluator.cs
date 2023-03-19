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
    public class SimilarityEvaluator : MonoBehaviour
    {
        private struct StructureBound
        {
            public Vector2 min;
            public Vector2 max;
        }

        [SerializeField] private OCRSettings m_ocrSettings = null;
        public Object scene;
        public Object inputDirectory;
        public Object outputDirectory;
        public Camera renderCamera;
        public Vector2Int targetSize;
        public float timeScale;
        public float stayDuration;
        public int margin;
        public MATERIALS targetMaterial;
        public bool outputErrorLevels;

        private List<Vector2> m_debugPos;

        private void Awake()
        {
            m_debugPos = new List<Vector2>();
            DontDestroyOnLoad(renderCamera.gameObject);
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            StartCoroutine(IE_TakeScreenshots());
        }
        private IEnumerator IE_TakeScreenshots()
        {
            string outputDirPath = Utils.GetAssetPath(outputDirectory);
            string inputDirPath = Utils.GetAssetPath(inputDirectory);
            string sceneName = Utils.GetAssetPath(scene);

            if (!Directory.Exists(outputDirPath))
            {
                Directory.CreateDirectory(outputDirPath);
            }

            timeScale = Mathf.Max(0.1f, timeScale);

            Time.timeScale = timeScale;
            string[] filepaths = Directory.GetFiles(inputDirPath, "*.xml");
            string[] xlevels = new string[filepaths.Length];
            for (int i = 0; i < xlevels.Length; i++)
                xlevels[i] = File.ReadAllText(filepaths[i]);
            LevelList.Instance.LoadLevelsFromSource(xlevels);

            for (int i = 0; i < xlevels.Length; i++)
            {
                WaitForEndOfFrame wff = new WaitForEndOfFrame();

                //load level
                yield return wff;
                LevelList.Instance.CurrentIndex = i;
                yield return wff;
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                yield return wff;
                //load level

                //take a screenshot
                yield return new WaitForSeconds(stayDuration);
                Rect sBound = GetStructureBound();

                if (sBound.size.x <= 0 || sBound.size.y <= 0) //can't find any block
                {
                    if (outputErrorLevels)
                    {
                        Vector2 tMinPos = new Vector2(renderCamera.transform.position.x - 10, renderCamera.transform.position.y - 10);
                        Vector2 tMaxPos = new Vector2(renderCamera.transform.position.x + 10, renderCamera.transform.position.y + 10);
                        sBound = Rect.MinMaxRect(tMinPos.x, tMinPos.y, tMaxPos.x, tMaxPos.y);
                    }
                    else
                    {
                        continue;
                    }
                }

                RenderTexture tTexture = CreateRenderTexture();
                m_debugPos.Clear();
                m_debugPos.Add(sBound.min);
                m_debugPos.Add(sBound.max);

                renderCamera.targetTexture = tTexture;

                AdjustBlocks();
                AdjustCamera(sBound);

                yield return new WaitForEndOfFrame();

                string fileName = string.Format("{0}.png", Path.GetFileNameWithoutExtension(filepaths[i]));
                string filePath = Path.Combine(outputDirPath, fileName);
                for (int j = 0; j < 7; j++)
                {
                    yield return wff;
                }
                SaveTexture(tTexture, filePath);
                //take a screenshot

                //unload level
                renderCamera.targetTexture = null;
                tTexture.Release();
                yield return SceneManager.UnloadSceneAsync(sceneName);
                yield return wff;
                //unload level
            }
            Time.timeScale = 1.0f;
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Debug.Log("============== similarity check is done ==============");
        }

        private void AdjustBlocks()
        {
            List<ABBlock> blocks = new List<ABBlock>(FindObjectsOfType<ABBlock>());
            foreach (ABBlock b in blocks)
            {
                string blockType = Utils.GetBlockType(b.gameObject);
                if (!string.IsNullOrEmpty(blockType))
                {
                    Sprite ocrSprite = m_ocrSettings.GetOCRSprite(blockType);
                    if (ocrSprite != null)
                    {
                        for (int i = 0; i < b._woodSprites.Length; i++)
                        {
                            b._woodSprites[i] = ocrSprite;
                        }
                        for (int i = 0; i < b._stoneSprites.Length; i++)
                        {
                            b._stoneSprites[i] = ocrSprite;
                        }
                        for (int i = 0; i < b._iceSprites.Length; i++)
                        {
                            b._iceSprites[i] = ocrSprite;
                        }
                        b.SetMaterial(b._material);
                    }
                }
            }
        }

        private void AdjustCamera(Rect structureBound)
        {
            ABGameplayCamera gameCam = FindObjectOfType<ABGameplayCamera>();
            renderCamera.transform.position = new Vector3(structureBound.center.x, structureBound.center.y, gameCam.transform.position.z);

            float structureAspect = structureBound.width / structureBound.height;
            if (structureAspect > renderCamera.aspect)
            {
                float tWidth = renderCamera.aspect * structureBound.height;
                float targetWidth = structureBound.width;
                float mod = targetWidth / tWidth;
                renderCamera.orthographicSize = structureBound.height * 0.5f * mod;
            }
            else
            {
                renderCamera.orthographicSize = structureBound.height * 0.5f;
            }
        }

        private RenderTexture CreateRenderTexture()
        {
            return new RenderTexture(targetSize.x - (margin * 2), targetSize.y - (margin * 2), 16, RenderTextureFormat.ARGB32);
        }

        private Rect GetStructureBound()
        {
            try
            {
                Vector2 minPos = Vector2.positiveInfinity;
                Vector2 maxPos = Vector2.negativeInfinity;
                List<ABBlock> blocks = new List<ABBlock>(FindObjectsOfType<ABBlock>());
                foreach (ABBlock b in blocks)
                {
                    Collider2D col = b.GetComponent<Collider2D>();
                    minPos.x = Mathf.Min(minPos.x, col.bounds.min.x);
                    minPos.y = Mathf.Min(minPos.y, col.bounds.min.y);
                    maxPos.x = Mathf.Max(maxPos.x, col.bounds.max.x);
                    maxPos.y = Mathf.Max(maxPos.y, col.bounds.max.y);
                }
                return Rect.MinMaxRect(minPos.x, minPos.y, maxPos.x, maxPos.y);
            }
            catch { }
            return Rect.zero;
        }

        private void Update()
        {
            if (m_debugPos != null && m_debugPos.Count >= 2)
            {
                Debug.DrawLine(m_debugPos[0], m_debugPos[1], Color.magenta);
            }
        }

        public void SaveTexture(RenderTexture renderTexture, string filePath)
        {
            byte[] bytes = CreateTexture2D(renderTexture).EncodeToPNG();
            File.WriteAllBytes(filePath, bytes);
        }

        private Texture2D CreateTexture2D(RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width + (margin * 2), rTex.height + (margin * 2), TextureFormat.RGBA32, false);
            //set to camera background color
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    tex.SetPixel(x, y, renderCamera.backgroundColor);
                }
            }

            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), margin, margin);
            tex.Apply();
            return tex;
        }
    }
}