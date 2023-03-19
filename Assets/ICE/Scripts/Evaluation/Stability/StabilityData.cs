using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICE.Evaluation
{
    [Serializable]
    public class StabilityData
    {
        public int dataCount;
        public float rate;
        public List<Stability> raws;

        public StabilityData()
        {
            dataCount = 0;
            rate = 0;
            raws = new List<Stability>();
        }
    }
}