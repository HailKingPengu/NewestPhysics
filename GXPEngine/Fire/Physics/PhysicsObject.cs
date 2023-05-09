using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

public class PhysicsObject : GameObject
{
    public Vec2 position;
    public float restitution;
    public Vec2 velocity;
    public float mass;
    public float inverseMass;

    public PhysicsObject(float restitution, float mass)
    {
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

    void ResolveCollision(CollisionData data)
    {
        // Calculate relative velocity 
        Vec2 rv = data.b.velocity - data.a.velocity;
        // Calculate relative velocity in terms of the normal direction 
        float velAlongNormal = Vec2.Dot(rv, data.normal);
        // Do not resolve if velocities are separating 
        if (velAlongNormal > 0)
            return;
        // Calculate restitution 
        float e = Mathf.Min(data.a.restitution, data.b.restitution);
        // Calculate impulse scalar 
        float j = -(1 + e) * velAlongNormal;
        j /= data.a.inverseMass + data.b.inverseMass;
        // Apply impulse 
        Vec2 impulse = j * data.normal;
        data.a.velocity -= data.a.inverseMass * impulse;
        data.b.velocity += data.b.inverseMass * impulse;
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
