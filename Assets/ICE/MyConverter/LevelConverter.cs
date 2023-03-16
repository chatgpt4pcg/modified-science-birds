using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICE
{
    public static class LevelConverter
    {
        public static readonly Vector2 STRUCTURE_MIN_POS = new Vector2(2.0f, -3.5f);

        public const float LV_WIDTH = 18.0f;
        public const short BIRD_COUNT = 1;
        public static readonly Vector2 SLINGSHOT_POS = new Vector2(-9.0f, -2.5f);

        private static ObjectDataset m_objDataset;
        public static ObjectDataset ObjDataset
        {
            get
            {
                if (m_objDataset == null)
                {
                    m_objDataset = Resources.Load<ObjectDataset>("ObjectDataset");
                }
                return m_objDataset;
            }
        }

        public static ABLevel ToLevel(string gptString)
        {
            ABLevel val = new ABLevel();
            val.blocks = new List<BlockData>();
            gptString = ClearWhiteSpace(gptString);
            List<string> lines = GetLines(gptString);
            ObjectDataset dataset = Resources.Load<ObjectDataset>("ObjectDataset");
            foreach (string l in lines)
            {
                string tName = GetObjectName(l);
                Vector2 tPos = GetObjectPosition(l);
                float tRot = 0;
                string tMat = MATERIALS.wood.ToString();
                Vector2 tSize = dataset.GetObjectSize(tName);
                tPos -= (tSize * 0.5f);

                val.blocks.Add(new BlockData(tName,tRot, tPos.x,tPos.y,tMat));
            }
            
            Rect lvBoundary = GetLevelBoundary(val);
            Vector2 shiftVal = STRUCTURE_MIN_POS - lvBoundary.min;
            val.blocks = ShiftObjects<BlockData>(val.blocks, shiftVal);

            //..define level's birds
            val.birds = new List<BirdData>();
            for (int i = 0; i < BIRD_COUNT; i++)
                val.birds.Add(new BirdData("" + BIRDS.BirdRed));

            //..define level's slingshot
            val.slingshot = new SlingData(SLINGSHOT_POS.x, SLINGSHOT_POS.y);


            Vector2 levelsize = lvBoundary.size;
            float totalwidth = Mathf.Abs(STRUCTURE_MIN_POS.x) + levelsize.x;
            float totalheight = Mathf.Abs(STRUCTURE_MIN_POS.y) + levelsize.y;

            //..define level's width
            val.width = (int)(totalwidth / LV_WIDTH) + 3;

            //..define camera
            float camaspectratio = Camera.main != null ? Camera.main.aspect : 1.6f;
            float camsize = Mathf.Max(totalheight * camaspectratio, totalwidth) * 2.0f; //..pick maximum size depending on level's width or height
            camsize = Mathf.Max(camsize, 17.0f);
            val.camera = new CameraData(camsize, camsize + 5.0f, STRUCTURE_MIN_POS.x, 0);

            return val;
        }

        public static string ClearWhiteSpace(string message)
        {
            return message.Replace(" ", "");
        }

        public static List<string> GetLines(string message)
        {
            return new List<string>(message.Split(new string[] { "\n", System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries));
        }

        private static string GetObjectName(string line)
        {
            try
            {
                int sIndex = line.IndexOf("\"") + 1;
                int eIndex = line.IndexOf("\"", sIndex);
                return line.Substring(sIndex, eIndex - sIndex);
            }
            catch (System.Exception err) 
            {
                Debug.LogWarning(err.ToString());
            }
            return null;
        }

        private static Vector2 GetObjectPosition(string line)
        {
            try
            {
                Vector2 val = new Vector3();
                int sIndex = line.IndexOf(",(") + 2;
                int eIndex = line.IndexOf(")", sIndex);
                string valString = line.Substring(sIndex, eIndex - sIndex);
                string[] splitted = valString.Split(new string[] { ","}, System.StringSplitOptions.RemoveEmptyEntries);
                val.x = (float)System.Convert.ToDouble(splitted[0]);
                val.y = (float)System.Convert.ToDouble(splitted[1]);
                return val;
            }
            catch (System.Exception err)
            {
                Debug.LogWarning(err.ToString());
            }
            return new Vector3();
        }

        public static Rect GetLevelBoundary(ABLevel level)
        {
            try
            {
                //TODO: Rotation
                Vector2 minPos = Vector2.positiveInfinity;
                Vector2 maxPos = Vector2.negativeInfinity;
                foreach (BlockData b in level.blocks)
                {
                    Vector2 tHalfSize = GetSize(b.type, b.rotation) * 0.5f;
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

        public static Vector2 GetSize(string blockName, float rotation)
        {
            Vector2 tSize = ObjDataset.GetObjectSize(blockName);
            return rotation == 0 ? tSize : new Vector2(tSize.y, tSize.x);
        }
    }
}