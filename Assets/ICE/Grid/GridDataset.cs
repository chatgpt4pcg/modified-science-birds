using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICE
{
    [CreateAssetMenu(menuName = ("ICE/GridDataset"))]
    public class GridDataset : ScriptableObject
    {
        [System.Serializable]
        public class ObjectData
        {
            [SerializeField] private string m_id;
            [SerializeField] private string m_blockName;
            [SerializeField] private float m_blockRotation;

            public string Id => m_id;
            public string BlockName => m_blockName;
            public float BlockRotation => m_blockRotation;
        }

        public float cellWidth;
        public List<ObjectData> data;
    }
}