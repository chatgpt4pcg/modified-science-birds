using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ICE.Evaluation
{
    [Serializable]
    public class Stability
    {
        public string tag;
        public float score;
        public int startingObjects;
        public int movingObjects;

        public Stability()
        {
            tag = "";
            score = 0;
            startingObjects = 0;
            movingObjects = 0;
        }

        public Stability Clone()
        {
            return new Stability()
            {
                tag = this.tag,
                score = this.score,
                startingObjects = this.startingObjects,
                movingObjects = this.movingObjects
            };
        }
    }
}