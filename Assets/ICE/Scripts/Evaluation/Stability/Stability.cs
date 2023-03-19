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
        public bool isStable;

        public Stability()
        {
            tag = "";
            isStable = false;
        }

        public Stability Clone()
        {
            return new Stability()
            {
                tag = this.tag,
                isStable = this.isStable
            };
        }
    }
}