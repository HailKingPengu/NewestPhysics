using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct AABB
{
    public Vec2 min;
    public Vec2 max;

    public static bool AABBvsAABB(AABB a, AABB b)
    {
        if (a.max.x < b.min.x && a.min.x > b.max.x) return false;
        if (a.max.y < b.min.y && a.min.y > b.max.y) return false;

        return true;
    }
}

public struct Circle
{
    public float radius;
    public Vec2 position;

    public static bool CirclevsCircleOptimized(Circle a, Circle b)
    {
        float r = a.radius + b.radius;
        r *= r;
        return r < (Mathf.Pow((a.position.x + b.position.x), 2) + Mathf.Pow((a.position.y + b.position.y),2));
    }
}




