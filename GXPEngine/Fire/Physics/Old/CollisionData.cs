using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A
{
    public struct CollisionData
    {
        public PhysicsObject a;
        public PhysicsObject b;
        public float penetration;
        public Vec2 normal;
        public bool isEmpty;
    }
}
