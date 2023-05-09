using GXPEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AABB : PhysicsObject
{
    public Vec2 min;
    public Vec2 max;
    public float width;
    public float height;
    public AABB(float restitution, float mass, Vec2 pos, Vec2 size) : base(restitution, mass, pos)
    {
        min.x = pos.x - (size.x / 2);
        min.y = pos.y - (size.y / 2);
        max.x = min.x + size.x;
        max.y = min.y + size.y;
        width = size.x;
        height = size.y;
        draw = new EasyDraw((int)width, (int)height);
        game.AddChild(draw);
    }

    public static bool AABBvsAABB(AABB a, AABB b)
    {
        if (a.max.x < b.min.x && a.min.x > b.max.x) return false;
        if (a.max.y < b.min.y && a.min.y > b.max.y) return false;

        return true;
    }

    public override void Draw()
    {
        draw.SetXY(position.x, position.y);
        //Console.WriteLine(draw.x + " " + draw.y);
        //Console.WriteLine(min.x + " " + min.y);
        draw.Fill(0,255,255);
        draw.Rect(min.x, min.y, width, height);
    }
}

public class Circle : PhysicsObject
{
    public float radius;
    public Vec2 center;

    public Circle(float restitution, float mass, Vec2 pos) : base(restitution, mass, pos)
    {

    }

    public static bool CirclevsCircleOptimized(Circle a, Circle b)
    {
        float r = a.radius + b.radius;
        r *= r;
        return r < (Mathf.Pow((a.position.x + b.position.x), 2) + Mathf.Pow((a.position.y + b.position.y), 2));
    }


}




