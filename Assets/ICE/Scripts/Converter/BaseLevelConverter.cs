using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICE
{
    [CreateAssetMenu(fileName = (nameof(BaseLevelConverter)), menuName = ("ICE/Level Converter/Base"))]
    public class BaseLevelConverter : ScriptableObject
    {
        public static readonly Vector2 STRUCTURE_MIN_POS = new Vector2(2.0f, -3.5f);

        public const float LV_WIDTH = 18.0f;
        public const short BIRD_COUNT = 3;
        public static readonly Vector2 SLINGSHOT_POS = new Vector2(-9.0f, -2.5f);

        [SerializeField] protected ConverterSettings m_settings;
        protected virtual string ResourcePath { get { return ""; } }

        public virtual ABLevel ToLevel(string gptString)
        {
            return null;
        }

        public static string ClearWhiteSpace(string message)
        {
            return message.Replace(" ", "");
        }

        public static List<string> GetLines(string message)
        {
            return new List<string>(message.Split(new string[] { "\n", System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries));
        }

        public static Rect GetLevelBoundary(ABLevel level, ConverterSettings settings)
        {
            try
            {
                //TODO: Rotation
                Vector2 minPos = Vector2.positiveInfinity;
                Vector2 maxPos = Vector2.negativeInfinity;
                foreach (BlockData b in level.blocks)
                {
                    Vector2 tHalfSize = settings.GetABObjectSize(b.type, b.rotation) * 0.5f;
                    minPos.x = Mathf.Min(minPos.x, b.x - tHalfSize.x);
                    minPos.y = Mathf.Min(minPos.y, b.y - tHalfSize.y);
                    maxPos.x = Mathf.Max(maxPos.x, b.x + tHalfSize.x);
                    maxPos.y = Mathf.Max(maxPos.y, b.y + tHalfSize.y);
                }
                return Rect.MinMaxRect(minPos.x, minPos.y, maxPos.x, maxPos.y);
            }
            catch { }
            return Rect.zero;
        }

        public static List<T> ShiftObjects<T>(List<T> objects, Vector2 shiftValue) where T : OBjData
        {
            foreach (T o in objects)
            {
                o.x += shiftValue.x;
                o.y += shiftValue.y;
            }
            return objects;
        }
    }
}