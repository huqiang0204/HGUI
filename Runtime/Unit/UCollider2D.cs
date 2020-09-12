using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Unit
{
    public enum CollisionType
    {
        None,
        Cricle,
        Pollygon,
        Line,
        Dot,
        Capsules
    }
    public class UCollider2D
    {
        public CollisionType type { get; protected set; }
        public Unit Target;
        public virtual void Update()
        {
        }
        public virtual bool Check(UCollider2D other)
        {
            return false;
        }
    }
}
