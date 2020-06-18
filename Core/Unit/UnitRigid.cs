using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace huqiang.Unit
{
    public class UnitRigid
    {
        public Unit Target;
        public float Gravity;
        public Vector2 Acceleration;
        public float DecayRateX = 0.998f;
        public void Update(float time)
        {
            float a = time * 1000;
            float r = DecayRateX;
            int t = (int)a;
            float d = (Mathf.Pow(r, t + 1) - r) / (r - 1);
            float l = Acceleration.x * d;
            
        }
    }
}
