using GXPEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Collision
{

    public static void Dispatch(Manifold m, Body a, Body b)
    {
        if(a.GetShapeType() is Shape.ShapeType.eCircle && b.GetShapeType() is Shape.ShapeType.eCircle)
        {
            CircletoCircle(m, a, b);
        }
        else if (a.GetShapeType() is Shape.ShapeType.eCircle && b.GetShapeType() is Shape.ShapeType.ePoly)
        {
            CircletoPolygon(m, a, b);
        }
        else if (a.GetShapeType() is Shape.ShapeType.ePoly && b.GetShapeType() is Shape.ShapeType.eCircle)
        {
            PolygontoCircle(m, a, b);
        }
        else if (a.GetShapeType() is Shape.ShapeType.ePoly && b.GetShapeType() is Shape.ShapeType.ePoly)
        {
            PolygontoPolygon(m, a, b);
        }
    }
    static void CircletoCircle(Manifold m, Body a, Body b)
    {
        Circle A = a.shape as Circle;
        Circle B = b.shape as Circle;

        // Calculate translational vector, which is normal
        Vec2 normal = b.position - a.position;

        float dist_sqr = normal.lengthSquared;
        float radius = A.radius + B.radius;

        // Not in contact
        if (dist_sqr >= radius * radius)
        {
            m.contact_count = 0;
            return;
        }

        float distance = Mathf.Sqrt(dist_sqr);

        m.contact_count = 1;

        if (distance == 0.0f)
        {
            m.penetration = A.radius;
            m.normal = new Vec2(1, 0);
            m.contacts[0] = a.position;
        }
        else
        {
            m.penetration = radius - distance;
            m.normal = normal / distance; // Faster than using Normalized since we already performed sqrt
            m.contacts[0] = m.normal * A.radius + a.position;
        }
    }

    static void CircletoPolygon(Manifold m, Body a, Body b)
    {
        Circle A = a.shape as Circle;
        PolygonShape B = b.shape as PolygonShape;

        m.contact_count = 0;

        // Transform circle center to Polygon model space
        Vec2 center = a.position;
        center = B.u.Transpose() * (center - b.position);

        // Find edge with minimum penetration
        // Exact concept as using support points in Polygon vs Polygon
        float separation = -float.MaxValue;
        uint faceNormal = 0;
        for (uint i = 0; i < B.m_vertexCount; ++i)
        {
            float s = Vec2.Dot(B.m_normals[i], center - B.m_vertices[i]);

            if (s > A.radius)
                return;

            if (s > separation)
            {
                separation = s;
                faceNormal = i;
            }
        }

        // Grab face's vertices
        Vec2 v1 = B.m_vertices[faceNormal];
        uint i2 = faceNormal + 1 < B.m_vertexCount ? faceNormal + 1 : 0;
        Vec2 v2 = B.m_vertices[i2];

        // Check to see if center is within polygon
        if (separation < 0.000001f)
        {
            m.contact_count = 1;
            m.normal = 0-(B.u * B.m_normals[faceNormal]);
            m.contacts[0] = m.normal * A.radius + a.position;
            m.penetration = A.radius;
            return;
        }

        // Determine which voronoi region of the edge center of circle lies within
        float dot1 = Vec2.Dot(center - v1, v2 - v1);
        float dot2 = Vec2.Dot(center - v2, v1 - v2);
        m.penetration = A.radius - separation;

        // Closest to v1
        if (dot1 <= 0.0f)
        {
            if (Mathf.Pow(Vec2.Distance(center, v1), 2) > A.radius * A.radius)
                return;

            m.contact_count = 1;
            Vec2 n = v1 - center;
            n = B.u * n;
            n.Normalize();
            m.normal = n;
            v1 = B.u * v1 + b.position;
            m.contacts[0] = v1;
        }

        // Closest to v2
        else if (dot2 <= 0.0f)
        {
            if (Mathf.Pow(Vec2.Distance(center, v2), 2) > A.radius * A.radius)
                return;

            m.contact_count = 1;
            Vec2 n = v2 - center;
            v2 = B.u * v2 + b.position;
            m.contacts[0] = v2;
            n = B.u * n;
            n.Normalize();
            m.normal = n;
        }

        // Closest to face
        else
        {
            Vec2 n = B.m_normals[faceNormal];
            if (Vec2.Dot(center - v1, n) > A.radius)
                return;

            n = B.u * n;
            m.normal = 0 - n;
            m.contacts[0] = m.normal * A.radius + a.position;
            m.contact_count = 1;
        }
    }

    static void PolygontoCircle(Manifold m, Body a, Body b)
    {
        CircletoPolygon(m, b, a);
        m.normal = 0 - m.normal;
    }

    static float FindAxisLeastPenetration(ref uint faceIndex, PolygonShape A, PolygonShape B)
    {
        float bestDistance = -float.MaxValue;
        uint bestIndex = 0;

        for (uint i = 0; i < A.m_vertexCount; ++i)
        {
            // Retrieve a face normal from A
            Vec2 n = A.m_normals[i];
            Vec2 nw = A.u * n;

            // Transform face normal into B's model space
            Mat2 buT = B.u.Transpose();
            n = buT * nw;

            // Retrieve support point from B along -n
            Vec2 s = B.GetSupport(0 - n);

            // Retrieve vertex on face from A, transform into
            // B's model space
            Vec2 v = A.m_vertices[i];
            v = A.u * v + A.body.position;
            v -= B.body.position;
            v = buT * v;

            // Compute penetration distance (in B's model space)
            float d = Vec2.Dot(n, s - v);

            // Store greatest distance
            if (d > bestDistance)
            {
                bestDistance = d;
                bestIndex = i;
            }
        }

        faceIndex = bestIndex;
        return bestDistance;
    }

    static void FindIncidentFace(Vec2[] v, PolygonShape RefPoly, PolygonShape IncPoly, uint referenceIndex)
    {
        Vec2 referenceNormal = RefPoly.m_normals[referenceIndex];

        // Calculate normal in incident's frame of reference
        referenceNormal = RefPoly.u * referenceNormal; // To world space
        referenceNormal = IncPoly.u.Transpose() * referenceNormal; // To incident's model space

        // Find most anti-normal face on incident polygon
        int incidentFace = 0;
        float minDot = float.MaxValue;
        for (int i = 0; i < IncPoly.m_vertexCount; ++i)
        {
            float dot = Vec2.Dot(referenceNormal, IncPoly.m_normals[i]);
            if (dot < minDot)
            {
                minDot = dot;
                incidentFace = i;
            }
        }

        // Assign face vertices for incidentFace
        v[0] = IncPoly.u * IncPoly.m_vertices[incidentFace] + IncPoly.body.position;
        incidentFace = incidentFace + 1 >= (int)IncPoly.m_vertexCount ? 0 : incidentFace + 1;
        v[1] = IncPoly.u * IncPoly.m_vertices[incidentFace] + IncPoly.body.position;
    }

    static uint Clip(Vec2 n, float c, Vec2[] face)
    {
        uint sp = 0;
        Vec2[] out1 = new Vec2[2] {
            face[0],
            face[1]
        };

        // Retrieve distances from each endpoint to the line
        // d = ax + by - c
        float d1 = Vec2.Dot(n, face[0]) - c;
        float d2 = Vec2.Dot(n, face[1]) - c;

        // If negative (behind plane) clip
        if (d1 <= 0.0f) out1[sp++] = face[0];
        if (d2 <= 0.0f) out1[sp++] = face[1];

        // If the points are on different sides of the plane
        if (d1 * d2 < 0.0f) // less than to ignore -0.0f
        {
            // Push interesection point
            float alpha = d1 / (d1 - d2);
    out1[sp] = face[0] + alpha * (face[1] - face[0]);
            ++sp;
        }

        // Assign our new converted values
        face[0] = out1[0];
        face[1] = out1[1];

        Debug.Assert(sp != 3);

        return sp;
    }

    static void PolygontoPolygon(Manifold m, Body a, Body b)
    {
        PolygonShape A = a.shape as PolygonShape;
        PolygonShape B = b.shape as PolygonShape;
        m.contact_count = 0;

        // Check for a separating axis with A's face planes
        uint faceA = 0;
        float penetrationA = FindAxisLeastPenetration(ref faceA, A, B);
        if (penetrationA >= 0.0f)
            return;

        // Check for a separating axis with B's face planes
        uint faceB = 0;
        float penetrationB = FindAxisLeastPenetration(ref faceB, B, A);
        if (penetrationB >= 0.0f)
            return;

        uint referenceIndex;
        bool flip; // Always point from a to b

        PolygonShape RefPoly; // Reference
        PolygonShape IncPoly; // Incident

        // Determine which shape contains reference face
        if (BiasGreaterThan(penetrationA, penetrationB))
        {
            RefPoly = A;
            IncPoly = B;
            referenceIndex = faceA;
            flip = false;
        }

        else
        {
            RefPoly = B;
            IncPoly = A;
            referenceIndex = faceB;
            flip = true;
        }

        // World space incident face
        Vec2[] incidentFace = new Vec2[2];
        FindIncidentFace(incidentFace, RefPoly, IncPoly, referenceIndex);

        //        y
        //        ^  .n       ^
        //      +---c ------posPlane--
        //  x < | i |\
        //      +---+ c-----negPlane--
        //             \       v
        //              r
        //
        //  r : reference face
        //  i : incident poly
        //  c : clipped point
        //  n : incident normal

        // Setup reference face vertices
        Vec2 v1 = RefPoly.m_vertices[referenceIndex];
        referenceIndex = referenceIndex + 1 == RefPoly.m_vertexCount ? 0 : referenceIndex + 1;
        Vec2 v2 = RefPoly.m_vertices[referenceIndex];

        // Transform vertices to world space
        v1 = RefPoly.u * v1 + RefPoly.body.position;
        v2 = RefPoly.u * v2 + RefPoly.body.position;

        // Calculate reference face side normal in world space
        Vec2 sidePlaneNormal = (v2 - v1);
        sidePlaneNormal.Normalize();

        // Orthogonalize
        Vec2 refFaceNormal = new Vec2(sidePlaneNormal.y, -sidePlaneNormal.x );

        // ax + by = c
        // c is distance from origin
        float refC = Vec2.Dot(refFaceNormal, v1);
        float negSide = -Vec2.Dot(sidePlaneNormal, v1);
        float posSide = Vec2.Dot(sidePlaneNormal, v2);

        // Clip incident face to reference face side planes
        if (Clip(0-sidePlaneNormal, negSide, incidentFace) < 2)
            return; // Due to floating point error, possible to not have required points

        if (Clip(sidePlaneNormal, posSide, incidentFace) < 2)
            return; // Due to floating point error, possible to not have required points

        // Flip
        m.normal = flip ? 0 - refFaceNormal : refFaceNormal;

        // Keep points behind reference face
        uint cp = 0; // clipped points behind reference face
        float separation = Vec2.Dot(refFaceNormal, incidentFace[0]) - refC;
        if (separation <= 0.0f)
        {
            m.contacts[cp] = incidentFace[0];
            m.penetration = -separation;
            ++cp;
        }
        else
            m.penetration = 0;

        separation = Vec2.Dot(refFaceNormal, incidentFace[1]) - refC;
        if (separation <= 0.0f)
        {
            m.contacts[cp] = incidentFace[1];

            m.penetration += -separation;
            ++cp;

            // Average penetration
            m.penetration /= (float)cp;
        }

        m.contact_count = cp;
    }
    public static bool BiasGreaterThan(float a, float b)
    {
        const float k_biasRelative = 0.95f;
        const float k_biasAbsolute = 0.01f;
        return a >= b * k_biasRelative + a * k_biasAbsolute;
    }
}
