using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Body : Shape
{
    public Shape shape;

    public Vec2 position;
    public Vec2 velocity;

    public float angularVelocity;
    public float torque;
    public float orient; // radians

    public Vec2 force;

    // Set by shape
    public float I;  // moment of inertia
    public float iI; // inverse inertia
    public float m;  // mass
    public float im; // inverse masee

    // http://gamedev.tutsplus.com/tutorials/implementation/how-to-create-a-custom-2d-physics-engine-friction-scene-and-jump-table/
    public float staticFriction;
    public float dynamicFriction;
    public float restitution;
    public Body(Shape shape, int x, int y) : base("Checkers.png")
    {
        this.shape = shape;
        shape.body = this;
        position = new Vec2((float)x, (float)y);
        velocity = new Vec2(0, 0);
        angularVelocity = 0;
        torque = 0;
        orient = Mathf.NextFloat(-Mathf.PI, Mathf.PI);
        force = new Vec2(0, 0);
        staticFriction = 0.5f;
        dynamicFriction = 0.3f;
        restitution = 0.2f;
        shape.Initialize();
        this.position = new Vec2(x, y);
    }

    public void ApplyForce(Vec2 f)
    {
        force += f;
    }

    public void ApplyImpulse(Vec2 impulse, Vec2 contactVector)
    {
        velocity += im * impulse;
        angularVelocity += iI * Vec2.Cross(contactVector, impulse);
    }

    public void SetStatic()
    {
        I = 0.0f;
        iI = 0.0f;
        m = 0.0f;
        im = 0.0f;
    }

    public override void SetOrient(float radians)
    {
        orient = radians;
        shape.SetOrient(radians);
    }



    // Shape interface

}



