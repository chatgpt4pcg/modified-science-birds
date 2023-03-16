using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICE
{
    public static class Grid2LevelConverterIII
    {
        private static GridDataset m_gridDataset;
        private static GridDataset GrDataset
        {
            get
            {
                if (m_gridDataset == null)
                {
                    m_gridDataset = Resources.Load<GridDataset>("GridDatasetIII");
                }
                return m_gridDataset;
            }
        }

        private const string FUNCTION_TAG = "ab_drop(";
        private const string GRID_ID_OPEN_TAG = "\'";
        private const string GRID_ID_CLOSE_TAG = "\'";
        private const string CELL_ID_OPEN_TAG = "\',";
        private const string CELL_ID_CLOSE_TAG = ")";

        public static ABLevel ToLevel(string gptString)
        {
            ABLevel val = new ABLevel();
            val.blocks = new List<BlockData>();
            gptString = LevelConverter.ClearWhiteSpace(gptString);
            List<string> lines = LevelConverter.GetLines(gptString);
            foreach (string l in lines)
            {
                try
                {
                    if (l.IndexOf(FUNCTION_TAG) != 0)
                    {
                        continue;
                    }
                    string tId = GetGridObjId(l);
                    int tCellId = GetGridCellId(l);
                    GridDataset.ObjectData tData = GrDataset.data.Find(x => x.Id == tId);
                    string tName = tData.BlockName;
                    float tRot = tData.BlockRotation;
                    string tMat = MATERIALS.wood.ToString();

                    Vector2 tPos = GetObjectPosition(tCellId, tName, tRot, val.blocks);
                    val.blocks.Add(new BlockData(tName, tRot, tPos.x, tPos.y, tMat));
                }
                catch { }
            }

            Rect lvBoundary = LevelConverter.GetLevelBoundary(val);
            Vector2 shiftVal = LevelConverter.STRUCTURE_MIN_POS - lvBoundary.min;
            val.blocks = LevelConverter.ShiftObjects<BlockData>(val.blocks, shiftVal);

            //..define level's birds
            val.birds = new List<BirdData>();
            for (int i = 0; i < LevelConverter.BIRD_COUNT; i++)
                val.birds.Add(new BirdData("" + BIRDS.BirdRed));

            //..define level's slingshot
            val.slingshot = new SlingData(LevelConverter.SLINGSHOT_POS.x, LevelConverter.SLINGSHOT_POS.y);

            Vector2 levelsize = lvBoundary.size;
            float totalwidth = Mathf.Abs(LevelConverter.STRUCTURE_MIN_POS.x) + levelsize.x;
            float totalheight = Mathf.Abs(LevelConverter.STRUCTURE_MIN_POS.y) + levelsize.y;

            //..define level's width
            val.width = (int)(totalwidth / LevelConverter.LV_WIDTH) + 3;

            //..define camera
            float camaspectratio = Camera.main != null ? Camera.main.aspect : 1.6f;
            float camsize = Mathf.Max(totalheight * camaspectratio, totalwidth) * 2.0f; //..pick maximum size depending on level's width or height
            camsize = Mathf.Max(camsize, 17.0f);
            val.camera = new CameraData(camsize, camsize + 5.0f, LevelConverter.STRUCTURE_MIN_POS.x, 0);

            return val;
        }

        private static string GetGridObjId(string line)
        {
            try
            {
                int sIndex = line.IndexOf(GRID_ID_OPEN_TAG) + GRID_ID_OPEN_TAG.Length;
                int eIndex = line.IndexOf(GRID_ID_CLOSE_TAG, sIndex);
                return line.Substring(sIndex, eIndex - sIndex);
            }
            catch (System.Exception err)
            {
                Debug.LogWarning(err.ToString());
            }
            return null;
        }

        private static int GetGridCellId(string line)
        {
            try
            {
                int sIndex = line.IndexOf(CELL_ID_OPEN_TAG) + CELL_ID_OPEN_TAG.Length;
                int eIndex = line.IndexOf(CELL_ID_CLOSE_TAG, sIndex);
                string valString = line.Substring(sIndex, eIndex - sIndex);
                return System.Convert.ToInt32(valString);
            }
            catch { }
            return -1;
        }

        private static Vector2 GetObjectPosition(int cellId, string blockName, float blockRotation, List<BlockData> currentBlocks)
        {
            try
            {
                Vector2 val = Vector2.zero;
                val.x = CellCenter(cellId);
                val.y = GetYPos(blockName, blockRotation, cellId, currentBlocks);
                return val;
            }
            catch { }
            return Vector2.zero;
        }

        private static float CellCenter(int cellId)
        {
            return ((float)cellId + 0.5f) * GrDataset.cellWidth;
        }

        private static float CellLeft(int cellId)
        {
            return (float)cellId * GrDataset.cellWidth;
        }

        private static float GetYPos(string blockName, float blockRotation, int targetCell, List<BlockData> currentBlocks)
        {
            Vector2 tHalfSize = LevelConverter.GetSize(blockName, blockRotation) * 0.5f;
            float cellCenter = CellCenter(targetCell);

            Rect rayRect = Rect.MinMaxRect(cellCenter - tHalfSize.x, 0,
                cellCenter + tHalfSize.x, 9999);
            float val = tHalfSize.y;
            foreach (BlockData b in currentBlocks)
            {
                Vector2 bHalfSize = LevelConverter.GetSize(b.type, b.rotation) * 0.5f;
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