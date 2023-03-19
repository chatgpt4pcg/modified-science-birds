using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICE
{
    [CreateAssetMenu(fileName = (nameof(GridLevelConverter)), menuName = ("ICE/Level Converter/Grid"))]
    public class GridLevelConverter : BaseLevelConverter
    {
        public override ABLevel ToLevel(string gptString)
        {
            ABLevel val = new ABLevel();
            val.blocks = new List<BlockData>();
            gptString = ClearWhiteSpace(gptString);
            List<string> lines = GetLines(gptString);
            List<ConverterSettings.GridObjectData> gridData = new List<ConverterSettings.GridObjectData>(m_settings.GridData);
            foreach (string l in lines)
            {
                try
                {
                    if (l.IndexOf(m_settings.FunctionTag) != 0)
                    {
                        continue;
                    }
                    string tId = GetGridObjId(l);
                    int tCellId = GetGridCellId(l);
                    ConverterSettings.GridObjectData tData = gridData.Find(x => x.Id == tId);
                    string tName = tData.BlockName;
                    float tRot = tData.BlockRotation;
                    string tMat = MATERIALS.wood.ToString();

                    Vector2 tPos = GetObjectPosition(tCellId, tName, tRot, val.blocks);
                    val.blocks.Add(new BlockData(tName, tRot, tPos.x, tPos.y, tMat));
                }
                catch { }
            }

            Rect lvBoundary = GetLevelBoundary(val, m_settings);
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

        private string GetGridObjId(string line)
        {
            try
            {
                int sIndex = line.IndexOf(m_settings.GridIdOpenTag) + m_settings.GridIdOpenTag.Length;
                int eIndex = line.IndexOf(m_settings.GridIdOpenTag, sIndex);
                return line.Substring(sIndex, eIndex - sIndex);
            }
            catch (System.Exception err)
            {
                Debug.LogWarning(err.ToString());
            }
            return null;
        }

        private int GetGridCellId(string line)
        {
            try
            {
                int sIndex = line.IndexOf(m_settings.CellIdOpenTag) + m_settings.CellIdOpenTag.Length;
                int eIndex = line.IndexOf(m_settings.CellIdCloseTag, sIndex);
                string valString = line.Substring(sIndex, eIndex - sIndex);
                return System.Convert.ToInt32(valString);
            }
            catch { }
            return -1;
        }

        private Vector2 GetObjectPosition(int cellId, string blockName, float blockRotation, List<BlockData> currentBlocks)
        {
            try
            {
                Vector2 val = Vector2.zero;
                Vector2 tHalfSize = m_settings.GetABObjectSize(blockName, blockRotation) * 0.5f;
                val.x = m_settings.Pivot == ConverterSettings.ObjectPivot.Centered ? CellCenter(cellId) : CellLeft(cellId) + tHalfSize.x;
                val.y = GetYPos(blockName, blockRotation, cellId, currentBlocks);
                return val;
            }
            catch { }
            return Vector2.zero;
        }

        private float CellCenter(int cellId)
        {
            return ((float)cellId + 0.5f) * m_settings.CellWidth;
        }

        private float CellLeft(int cellId)
        {
            return (float)cellId * m_settings.CellWidth;
        }

        private float GetYPos(string blockName, float blockRotation, int targetCell, List<BlockData> currentBlocks)
        {
            Vector2 tHalfSize = m_settings.GetABObjectSize(blockName, blockRotation) * 0.5f;
            float val = tHalfSize.y;

            Rect rayRect = Rect.zero;
            if (m_settings.Pivot == ConverterSettings.ObjectPivot.Centered)
            {
                float cellCenter = CellCenter(targetCell);
                rayRect = Rect.MinMaxRect(cellCenter - tHalfSize.x, 0, cellCenter + tHalfSize.x, 9999);
            }
            else
            {
                float cellLeft = CellLeft(targetCell);
                rayRect = Rect.MinMaxRect(cellLeft, 0, cellLeft + (tHalfSize.x * 2.0f), 9999);
            }

            foreach (BlockData b in currentBlocks)
            {
                Vector2 bHalfSize = m_settings.GetABObjectSize(b.type, b.rotation) * 0.5f;
                Rect bRect = Rect.MinMaxRect(b.x - bHalfSize.x, b.y - bHalfSize.y,
                    b.x + bHalfSize.x, b.y + bHalfSize.y);
                float tVal = bRect.yMax + tHalfSize.y;
                if (bRect.Overlaps(rayRect) && tVal > val)
                {
                    val = tVal;
                }
            }
            return val;
        }

    }
}