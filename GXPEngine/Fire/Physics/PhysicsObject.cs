using GXPEngine;
using GXPEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

public class PhysicsObject : Sprite
{
    public EasyDraw draw;
    public Vec2 position;
    public float restitution;
    public Vec2 velocity;
    public float mass;
    public float inverseMass;

    public PhysicsObject(float restitution, float mass, Vec2 pos) : base("Checkers.png")
    {
        position = pos;
        this.restitution = restitution;
        this.mass = mass;
        if (mass == 0)
        {
            inverseMass = 0;
        }
        else
        {
            inverseMass = 1 / mass;
        }
    }

    public void Update()
    {
        Draw();
        SetXY(position.x, position.y);
    }

    public virtual void Draw()
    {

    }



    void PositionalCorrection(CollisionData data)
    {
        const float percent = 0.2f; // usually 20% to 80% 
        const float slop = 0.01f; // usually 0.01 to 0.1 
        Vec2 correction = (Mathf.Max(data.penetration - slop, 0.0f) / (data.a.inverseMass + data.b.inverseMass) * percent * data.normal);
        data.a.position -= data.a.inverseMass * correction;
        data.b.position += data.b.inverseMass * correction;
    }
}
