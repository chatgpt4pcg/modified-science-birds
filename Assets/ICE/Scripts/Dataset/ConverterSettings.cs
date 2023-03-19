using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICE
{
    [CreateAssetMenu(menuName = ("ICE/Settings/Converter Settings"))]
    public class ConverterSettings : ScriptableObject
    {
        public enum ObjectPivot
        {
            Centered,
            CenterLeft
        }

        [System.Serializable]
        public class ABObjectData
        {
            [SerializeField] private string m_name;
            [SerializeField] private Vector2 m_size;

            public string Name => m_name;
            public Vector2 Size => m_size;
        }

        [System.Serializable]
        public class GridObjectData
        {
            [SerializeField] private string m_id;
            [SerializeField] private string m_blockName;
            [SerializeField] private float m_blockRotation;

            public string Id => m_id;
            public string BlockName => m_blockName;
            public float BlockRotation => m_blockRotation;
        }

        [SerializeField] private float m_cellWidth;
        [SerializeField] private ObjectPivot m_pivot;
        [SerializeField] private string m_functionTag = "ab_drop(";
        [SerializeField] private string m_gridIdOpenTag = "\'";
        [SerializeField] private string m_gridIdCloseTag = "\'";
        [SerializeField] private string m_cellIdOpenTag = "\',";
        [SerializeField] private string m_cellIdCloseTag = ")";
        [SerializeField] private List<ABObjectData> m_abObjectData;
        [SerializeField] private List<GridObjectData> m_gridObjectData;

        public float CellWidth => m_cellWidth;
        public ObjectPivot Pivot => m_pivot;
        public string FunctionTag => m_functionTag;
        public string GridIdOpenTag => m_gridIdOpenTag;
        public string GridIdCloseTag => m_gridIdCloseTag;
        public string CellIdOpenTag => m_cellIdOpenTag;
        public string CellIdCloseTag => m_cellIdCloseTag;
        public IReadOnlyList<GridObjectData> GridData => m_gridObjectData;

        public Vector2 GetABObjectSize(string objectName, float rotation)
        {
            try
            {
                Vector2 tSize = m_abObjectData.Find(x => x.Name == objectName).Size;
                return rotation == 0 ? tSize : new Vector2(tSize.y, tSize.x);
            }
            catch { }
            return Vector2.zero;
        }
    }
}