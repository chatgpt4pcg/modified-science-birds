using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICE.Evaluation
{
    [CreateAssetMenu(fileName = nameof(OCRSettings), menuName = "ICE/Settings/OCR Settings")]
    public class OCRSettings : ScriptableObject
    {
        [System.Serializable]
        private class OCRSpriteData
        {
            [SerializeField] private string m_blockName;
            [SerializeField] private Sprite m_ocrSprite;

            public string BlockName => m_blockName;
            public Sprite OCRSprite => m_ocrSprite;
        }

        [SerializeField] private List<OCRSpriteData> m_ocrSpriteData;

        public Sprite GetOCRSprite(string blockName)
        {
            try
            {
                return m_ocrSpriteData.Find(x => x.BlockName == blockName).OCRSprite;
            }
            catch { }
            return null;
        }
    }
}