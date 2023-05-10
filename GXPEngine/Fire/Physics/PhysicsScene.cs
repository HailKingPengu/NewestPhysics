using GXPEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PhysicsScene
{
    public PhysicsScene(int iterations)
    {
        m_iterations = iterations;
    }
    int m_iterations;
    List<Body> bodies;
    List<Manifold> contacts;

    void IntegrateForces(Body b, float dt)
    {
        if (b.im == 0.0f)
            return;

        b.velocity += (b.force * b.im + Manifold.gravity) * (dt / 2.0f);
        b.angularVelocity += b.torque * b.iI * (dt / 2.0f);
    }

    void IntegrateVelocity(Body b, float dt)
    {
        if (b.im == 0.0f)
            return;

        b.position += b.velocity * dt;
        b.orient += b.angularVelocity * dt;
        b.SetOrient(b.orient);
        IntegrateForces(b, dt);
    }

    public void Step()
    {
        // Generate new collision info
        contacts.Clear();
        for (int i = 0; i < bodies.Count(); ++i)
        {
            Body A = bodies[i];

            for (int j = i + 1; j < bodies.Count(); ++j)
            {
                Body B = bodies[j];
                if (A.im == 0 && B.im == 0)
                    continue;
                Manifold m = new Manifold(A, B );
                m.Solve();
                if (m.contact_count > 0)
                    contacts.Append(m);
            }
        }

        // Integrate forces
        for (int i = 0; i < bodies.Count(); ++i)
            IntegrateForces(bodies[i], Time.deltaTime);

        // Initialize collision
        for (int i = 0; i < contacts.Count(); ++i)
            contacts[i].Initialize();

        // Solve collisions
        for (int j = 0; j < m_iterations; ++j)
            for (int i = 0; i < contacts.Count(); ++i)
                contacts[i].ApplyImpulse();

        // Integrate velocities
        for (int i = 0; i < bodies.Count(); ++i)
            IntegrateVelocity(bodies[i], Time.deltaTime);

        // Correct positions
        for (int i = 0; i < contacts.Count(); ++i)
            contacts[i].PositionalCorrection();

        // Clear all forces
        for (int i = 0; i < bodies.Count(); ++i)
        {
            Body b = bodies[i];
            b.force = new Vec2(0, 0);
            b.torque = 0;
        }
    }

    //void Render()
    //{
    //    for (int i = 0; i < bodies.Count(); ++i)
    //    {
    //        Body* b = bodies[i];
    //        b.shape.Draw();
    //    }

    //    glPointSize(4.0f);
    //    glBegin(GL_POINTS);
    //    glColor3f(1.0f, 0.0f, 0.0f);
    //    for (int i = 0; i < contacts.Count(); ++i)
    //    {
    //        Manifold & m = contacts[i];
    //        for (int j = 0; j < m.contact_count; ++j)
    //        {
    //            Vec2 c = m.contacts[j];
    //            glVertex2f(c.x, c.y);
    //        }
    //    }
    //    glEnd();
    //    glPointSize(1.0f);

    //    glBegin(GL_LINES);
    //    glColor3f(0.0f, 1.0f, 0.0f);
    //    for (int i = 0; i < contacts.Count(); ++i)
    //    {
    //        Manifold & m = contacts[i];
    //        Vec2 n = m.normal;
    //        for (int j = 0; j < m.contact_count; ++j)
    //        {
    //            Vec2 c = m.contacts[j];
    //            glVertex2f(c.x, c.y);
    //            n *= 0.75f;
    //            c += n;
    //            glVertex2f(c.x, c.y);
    //        }
    //    }
    //    glEnd();
    //}

    Body Add(Shape shape, int x, int y)
    {
        Body b = new Body(shape, x, y);
        bodies.Append(b);
        return b;
    }
}
