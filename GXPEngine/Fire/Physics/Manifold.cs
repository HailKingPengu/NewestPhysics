using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Manifold
{
    const float gravityScale = 5.0f;
    public static Vec2 gravity = new Vec2( 0, 10.0f * gravityScale );
    public Manifold(Body a, Body b)
    {
        A = a;
        B = b;
    }

    public Body A;
    public Body B;

    public float penetration;     // Depth of penetration from collision
    public Vec2 normal;          // From A to B
    public Vec2[] contacts = new Vec2[2];     // Points of contact during collision
    public uint contact_count; // Number of contacts that occured during collision
    public float e;               // Mixed restitution
    public float df;              // Mixed dynamic friction
    public float sf;              // Mixed static friction
    public void Solve()
    {
        Collision.Dispatch(this, A, B);
    }

    public void Initialize()
    {
        // Calculate average restitution
        e = Mathf.Min(A.restitution, B.restitution);

        // Calculate static and dynamic friction
        sf = Mathf.Sqrt(A.staticFriction * B.staticFriction);
        df = Mathf.Sqrt(A.dynamicFriction * B.dynamicFriction);

        for (uint i = 0; i < contact_count; ++i)
        {
            // Calculate radii from COM to contact
            Vec2 ra = contacts[i] - A.position;
            Vec2 rb = contacts[i] - B.position;

            Vec2 rv = B.velocity + Vec2.Cross(B.angularVelocity, rb) -
                      A.velocity - Vec2.Cross(A.angularVelocity, ra);


            // Determine if we should perform a resting collision or not
            // The idea is if the only thing moving this object is gravity,
            // then the collision should be performed without any restitution
            if (rv.lengthSquared < (Time.deltaTime * gravity).lengthSquared + 0.0001f)
                e = 0.0f;
        }
    }

    public void ApplyImpulse()
    {
        // Early out and positional correct if both objects have infinite mass
        if (Equals(A.im + B.im, 0))
        {
            InfiniteMassCorrection();
            return;
        }

        for (uint i = 0; i < contact_count; ++i)
        {
            // Calculate radii from COM to contact
            Vec2 ra = contacts[i] - A.position;
            Vec2 rb = contacts[i] - B.position;

            // Relative velocity
            Vec2 rv = B.velocity + Vec2.Cross(B.angularVelocity, rb) -
                      A.velocity - Vec2.Cross(A.angularVelocity, ra);

            // Relative velocity along the normal
            float contactVel = Vec2.Dot(rv, normal);

            // Do not resolve if velocities are separating
            if (contactVel > 0)
                return;

            float raCrossN = Vec2.Cross(ra, normal);
            float rbCrossN = Vec2.Cross(rb, normal);
            float invMassSum = A.im + B.im + Mathf.Pow(raCrossN, 2) * A.iI + Mathf.Pow(rbCrossN, 2) * B.iI;

            // Calculate impulse scalar
            float j = -(1.0f + e) * contactVel;
            j /= invMassSum;
            j /= (float)contact_count;

            // Apply impulse
            Vec2 impulse = normal * j;
            A.ApplyImpulse(0 - impulse, ra);
            B.ApplyImpulse(impulse, rb);

            // Friction impulse
            rv = B.velocity + Vec2.Cross(B.angularVelocity, rb) -
                 A.velocity - Vec2.Cross(A.angularVelocity, ra);

            Vec2 t = rv - (normal * Vec2.Dot(rv, normal));
            t.Normalize();

            // j tangent magnitude
            float jt = -Vec2.Dot(rv, t);
            jt /= invMassSum;
            jt /= (float)contact_count;

            // Don't apply tiny friction impulses
            if (Equals(jt, 0.0f))
                return;

            // Coulumb's law
            Vec2 tangentImpulse;
            if (Mathf.Abs(jt) < j * sf)
                tangentImpulse = t * jt;
            else
                tangentImpulse = t * -j * df;

            // Apply friction impulse
            A.ApplyImpulse(0-tangentImpulse, ra);
            B.ApplyImpulse(tangentImpulse, rb);
        }
    }

    public void PositionalCorrection()
    {
        const float k_slop = 0.05f; // Penetration allowance
        const float percent = 0.4f; // Penetration percentage to correct
        Vec2 correction = (Mathf.Max(penetration - k_slop, 0.0f) / (A.im + B.im)) * normal * percent;
        A.position -= correction * A.im;
        B.position += correction * B.im;
    }

    void InfiniteMassCorrection()
    {
        A.velocity = new Vec2(0, 0);
        B.velocity = new Vec2(0, 0);
    }
}


