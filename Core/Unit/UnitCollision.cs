using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace huqiang.Unit
{
  
    public class UnitCollision
    {
        struct Context
        {
            public Unit Self;
            public Unit Other;
        }
        struct Context1
        {
            public Unit Self;
            public UnitCarrier Other;
        }
        struct Context2
        {
            public UnitCarrier Self;
            public UnitCarrier Other;
        }
        static List<Context> contexts;
        static List<Context1> contexts1;
        static List<Context2> contexts2;
        public static void Add(Unit one, Unit other)
        {
            if (contexts == null)
                contexts = new List<Context>();
            Context context = new Context();
            context.Self = one;
            context.Other = other;
            contexts.Add(context);
        }
        public static void Add(Unit one, UnitCarrier other)
        {
            if (contexts1 == null)
                contexts1 = new List<Context1>();
            Context1 context = new Context1();
            context.Self = one;
            context.Other = other;
            contexts1.Add(context);
        }
        public static void Add(UnitCarrier one, UnitCarrier other)
        {
            if (contexts2 == null)
                contexts2 = new List<Context2>();
            Context2 context = new Context2();
            context.Self = one;
            context.Other = other;
            contexts2.Add(context);
        }
        public static void Clear()
        {
            if (contexts != null)
                contexts.Clear();
            if (contexts1 != null)
                contexts1.Clear();
            if (contexts2 != null)
                contexts2.Clear();
        }
        public static void Collision()
        {
            if(contexts1!=null)
            {
                CollisionContext1();
            }    
            if(contexts2!=null)
            {
                CollisionContext2();
            }
        }
        static void CollisionContext1()
        {
            for(int i=0;i<contexts1.Count;i++)
            {
                var item = contexts1[i];
                if(item.Self.active)
                {
                    switch(item.Self.collider.type)
                    {
                        case CollisionType.Cricle:
                            switch(item.Other.collisionType)
                            {
                                case CollisionType.Cricle:
                                    CircleToCircle(item.Self,item.Other.Units);
                                    break;
                                case CollisionType.Pollygon:
                                    CircleToPollygon(item.Self, item.Other.Units);
                                    break;
                                case CollisionType.Capsules:
                                    break;
                                case CollisionType.Line:
                                    break;
                            }
                            break;
                        case CollisionType.Pollygon:
                            switch (item.Other.collisionType)
                            {
                                case CollisionType.Cricle:
                                    break;
                                case CollisionType.Pollygon:
                                    break;
                                case CollisionType.Capsules:
                                    break;
                                case CollisionType.Line:
                                    break;
                            }
                            break;
                        case CollisionType.Capsules:
                            switch (item.Other.collisionType)
                            {
                                case CollisionType.Cricle:
                                    CapsulesToCircle(item.Self, item.Other.Units);
                                    break;
                                case CollisionType.Pollygon:
                                    CapsulesToPollygon(item.Self, item.Other.Units);
                                    break;
                                case CollisionType.Capsules:
                                    break;
                                case CollisionType.Line:
                                    break;
                            }
                            break;
                        case CollisionType.Line:
                            break;
                    }
                }
            }
        }
        static void CollisionContext2()
        {
            for (int i = 0; i < contexts2.Count; i++)
            {
                var item = contexts2[i];
                switch (item.Self.collisionType)
                {
                    case CollisionType.Cricle:
                        switch (item.Other.collisionType)
                        {
                            case CollisionType.Cricle:
                                CircleToCircle(item.Self.Units, item.Other.Units);
                                break;
                            case CollisionType.Pollygon:
                                CircleToPollygon(item.Self.Units, item.Other.Units);
                                break;
                            case CollisionType.Capsules:
                                break;
                            case CollisionType.Line:
                                break;
                        }
                        break;
                    case CollisionType.Pollygon:
                        switch (item.Other.collisionType)
                        {
                            case CollisionType.Cricle:
                                CircleToPollygon(item.Other.Units, item.Self.Units);
                                break;
                            case CollisionType.Pollygon:
                                PollygonToPollygon(item.Self.Units,item.Other.Units);
                                break;
                            case CollisionType.Capsules:
                                break;
                            case CollisionType.Line:
                                break;
                        }
                        break;
                    case CollisionType.Capsules:
                        switch (item.Other.collisionType)
                        {
                            case CollisionType.Cricle:
                                CircleToCapsules(item.Other.Units,item.Self.Units);
                                break;
                            case CollisionType.Pollygon:
                                PollygonToCapsules(item.Other.Units,item.Self.Units);
                                break;
                            case CollisionType.Capsules:
                                break;
                            case CollisionType.Line:
                                break;
                        }
                        break;
                    case CollisionType.Line:
                        break;
                }
            }
        }
        public static void CircleToCircle(Unit cir,  List<Unit> po)
        {
            var cc = cir.collider as UCircleCollider;
            if (cc == null)
                return;
            for (int j = 0; j < po.Count; j++)
            {
                var b = po[j];
                if (b != null)
                {
                    if (b.active)
                    {
                        if (b.show)
                        {
                            var p = b.collider as UCircleCollider;
                            if (p != null)
                            {
                                if (huqiang.Physics2D.CircleToCircle(cir.localPosition, b.localPosition, cc.rad, p.rad))
                                {
                                    cir.OnCollision(b);
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void CircleToCircle(List<Unit> cir, List<Unit> po)
        {
            for (int i = 0; i < cir.Count; i++)
            {
                var a = cir[i];
                if (a != null)
                {
                    if (a.active)
                    {
                        if (a.show)
                        {
                            var col = a.collider as UCircleCollider;
                            if (col != null)
                            {
                                for (int j = 0; j < po.Count; j++)
                                {
                                    var b = po[j];
                                    if (b != null)
                                    {
                                        if (b.active)
                                        {
                                            if (b.show)
                                            {
                                                var p = b.collider as UCircleCollider;
                                                if (p != null)
                                                {
                                                    if (huqiang.Physics2D.CircleToCircle(a.localPosition, b.localPosition, col.rad, p.rad))
                                                    {
                                                        a.OnCollision(b);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void CircleToPollygon(Unit cir, List<Unit> po)
        {
            var cc = cir.collider as UCircleCollider;
            if (cc == null)
                return;
            for (int j = 0; j < po.Count; j++)
            {
                var b = po[j];
                if (b != null)
                {
                    if (b.active)
                    {
                        if (b.show)
                        {
                            var p = b.collider as UPolygonCollider;
                            if (p != null)
                            {
                                if (p.buf != null)
                                    if (huqiang.Physics2D.CircleToPolygon(cir.localPosition, cc.rad, p.buf))
                                    {
                                        cir.OnCollision(b);
                                    }
                            }
                        }
                    }
                }
            }
        }
        public static void CircleToPollygon(List<Unit> cir, List<Unit> po)
        {
            for (int i = 0; i < cir.Count; i++)
            {
                var a = cir[i];
                if (a != null)
                {
                    if (a.active)
                    {
                        if (a.show)
                        {
                            var col = a.collider as UCircleCollider;
                            if (col != null)
                            {
                                for (int j = 0; j < po.Count; j++)
                                {
                                    var b = po[j];
                                    if (b != null)
                                    {
                                        if (b.active)
                                        {
                                            if (b.show)
                                            {
                                                var p = b.collider as UPolygonCollider;
                                                if (p != null)
                                                {
                                                    if (p.buf != null)
                                                        if (huqiang.Physics2D.CircleToPolygon(a.localPosition, col.rad, p.buf))
                                                        {
                                                            a.OnCollision(b);
                                                        }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void PollygonToPollygon(List<Unit> cir, List<Unit> po)
        {
            for (int i = 0; i < cir.Count; i++)
            {
                var a = cir[i];
                if (a != null)
                {
                    if (a.active)
                    {
                        if (a.show)
                        {
                            var col = a.collider as UPolygonCollider;
                            if (col != null)
                            {
                                if (col.buf != null)
                                {
                                    for (int j = 0; j < po.Count; j++)
                                    {
                                        var b = po[j];
                                        if (b != null)
                                        {
                                            if (b.active)
                                            {
                                                if (b.show)
                                                {
                                                    var p = b.collider as UPolygonCollider;
                                                    if (p != null)
                                                    {
                                                        if (p.buf != null)
                                                            if (huqiang.Physics2D.PToP2(col.buf, p.buf))
                                                            {
                                                                a.OnCollision(b);
                                                            }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void CircleToCapsules(List<Unit> cir, List<Unit> po)
        {
            for (int i = 0; i < cir.Count; i++)
            {
                var a = cir[i];
                if (a != null)
                {
                    if (a.active)
                    {
                        if (a.show)
                        {
                            var col = a.collider as UCircleCollider;
                            if (col != null)
                            {
                                for (int j = 0; j < po.Count; j++)
                                {
                                    var b = po[j];
                                    if (b != null)
                                    {
                                        if (b.active)
                                        {
                                            if (b.show)
                                            {
                                                var p = b.collider as UCapsulesCollider;
                                                if (p != null)
                                                {
                                                    if (p.circle)
                                                    {
                                                        if (huqiang.Physics2D.CircleToCircle(a.localPosition, b.localPosition, col.radius, p.size.x))
                                                        {
                                                            a.OnCollision(b);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (huqiang.Physics2D.CircleToCircle(a.localPosition, p.top, col.radius, p.size.x))
                                                        {
                                                            a.OnCollision(b);
                                                        }
                                                        else if (huqiang.Physics2D.CircleToCircle(a.localPosition, p.down, col.radius, p.size.x))
                                                        {
                                                            a.OnCollision(b);
                                                        }
                                                        else if (huqiang.Physics2D.CircleToPolygon(a.localPosition, col.radius, p.buf))
                                                        {
                                                            a.OnCollision(b);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void PollygonToCapsules(List<Unit> cir, List<Unit> po)
        {
            for (int i = 0; i < cir.Count; i++)
            {
                var a = cir[i];
                if (a != null)
                {
                    if (a.active)
                    {
                        if (a.show)
                        {
                            var col = a.collider as UPolygonCollider;
                            if (col != null)
                            {
                                if (col.buf != null)
                                {
                                    for (int j = 0; j < po.Count; j++)
                                    {
                                        var b = po[j];
                                        if (b != null)
                                        {
                                            if (b.active)
                                            {
                                                if (b.show)
                                                {
                                                    var p = b.collider as UCapsulesCollider;
                                                    if (p != null)
                                                    {
                                                        if (p.circle)
                                                        {
                                                            if (huqiang.Physics2D.CircleToPolygon(b.localPosition, p.size.x, p.buf))
                                                            {
                                                                a.OnCollision(b);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (huqiang.Physics2D.CircleToPolygon(p.top, p.size.x, col.buf))
                                                            {
                                                                a.OnCollision(b);
                                                            }
                                                            else if (huqiang.Physics2D.CircleToPolygon(p.down, p.size.x, col.buf))
                                                            {
                                                                a.OnCollision(b);
                                                            }
                                                            else if (huqiang.Physics2D.PToP2(p.buf, col.buf))
                                                            {
                                                                a.OnCollision(b);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void CapsulesToCircle(Unit a,List<Unit> po)
        {
            var ori = a.collider as UCapsulesCollider;
            if (ori == null)
                return;
            for (int j = 0; j < po.Count; j++)
            {
                var b = po[j];
                if (b != null)
                {
                    if (b.active)
                    {
                        if (b.show)
                        {
                            var p = b.collider as UCircleCollider;
                            if (p != null)
                            {
                                if (ori.circle)
                                {
                                    if (huqiang.Physics2D.CircleToCircle(a.localPosition, b.localPosition, ori.radius, p.rad))
                                    {
                                        a.OnCollision(b);
                                    }
                                }
                                else
                                {
                                    if (huqiang.Physics2D.CircleToCircle(ori.top, b.localPosition, ori.radius, p.rad))
                                    {
                                        a.OnCollision(b);
                                    }
                                    else if (huqiang.Physics2D.CircleToCircle(ori.down, b.localPosition, ori.radius, p.rad))
                                    {
                                        a.OnCollision(b);
                                    }
                                    else if (huqiang.Physics2D.CircleToPolygon(b.localPosition, p.rad, ori.buf))
                                    {
                                        a.OnCollision(b);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void CapsulesToPollygon(Unit a, List<Unit> po)
        {
            var ori = a.collider as UCapsulesCollider;
            if (ori == null)
                return;
            for (int j = 0; j < po.Count; j++)
            {
                var b = po[j];
                if (b != null)
                {
                    if (b.active)
                    {
                        if (b.show)
                        {
                            var p = b.collider as UPolygonCollider;
                            if (p != null)
                            {
                                if (ori.circle)
                                {
                                    if (huqiang.Physics2D.CircleToPolygon(a.localPosition, ori.radius, p.buf))
                                    {
                                        a.OnCollision(b);
                                    }
                                }
                                else
                                {
                                    if (huqiang.Physics2D.CircleToPolygon(ori.top, ori.radius, p.buf))
                                    {
                                        a.OnCollision(b);
                                    }
                                    else if (huqiang.Physics2D.CircleToPolygon(ori.down, ori.radius, p.buf))
                                    {
                                        a.OnCollision(b);
                                    }
                                    else if (huqiang.Physics2D.PToP2(ori.buf, p.buf))
                                    {
                                        a.OnCollision(b);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
