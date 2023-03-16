using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICE
{
    [CreateAssetMenu(menuName = ("ICE/ObjectDataset"))]
    public class ObjectDataset : ScriptableObject
    {
        [System.Serializable]
        public class ObjectData
        {
            [SerializeField] private string m_name;
            [SerializeField] private Vector2 m_size;

            public string Name => m_name;
            public Vector2 Size => m_size;
        }

        [SerializeField] private List<ObjectData> m_data;

        public Vector2 GetObjectSize(string objectName)
        {
            try 
            {
                return m_data.Find(x => x.Name == objectName).Size;
            }
            catch { }
            return Vector2.zero;
        }
    }
}