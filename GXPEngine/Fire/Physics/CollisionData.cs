using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public struct CollisionData
{
    public PhysicsObject a;
    public PhysicsObject b;
    public float penetration;
    public Vec2 normal;
}
