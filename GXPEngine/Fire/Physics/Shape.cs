using GXPEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;


public class Shape : AnimationSprite
{
    public bool init = false;
    public enum ShapeType
    {
        eCircle,
        ePoly,
        eCount
    };
    public virtual Shape Clone() { return this; }
    public virtual void Initialize() { }
    public virtual void ComputeMass(float density) { }
    public virtual void SetOrient(float radians) { }
    public virtual void Draw() { }
    public virtual ShapeType GetShapeType()
    { if (body != null) return body.shape.GetShapeType(); else return ShapeType.eCount; }

    public Body body;

    // For circle shape
    public float radius;

    // For Polygon shape
    public Mat2 u; // Orientation matrix from model to world

    public Shape(string filename, int frames = -1, bool keepInCache = false, bool addCollider = false) : base(filename, 1, 1, frames, keepInCache, false)
    {

    }
}

public class Circle : Shape
{
    public Circle(float r) : base("circle.png")
    {
        radius = r;
        width = 2 * (int)r;
        height = 2 * (int)r;

    }

    public override Shape Clone()
    {
        return new Circle(radius);
    }

    public override void Initialize()
    {
        //body.width = 2 * (int)radius;
        //body.height = 2 * (int)radius;
        ComputeMass(1.0f);
        init = true;
    }

    public override void ComputeMass(float density)
    {
        body.m = Mathf.PI * radius * radius * density;
        body.im = (body.m != 0f) ? 1.0f / body.m : 0.0f;
        body.I = body.m * radius * radius;
        body.iI = (body.I != 0f) ? 1.0f / body.I : 0.0f;
    }

    public override void SetOrient(float radians)
    {
    }

    public override ShapeType GetShapeType()
    {
        if (!init)
            return ShapeType.eCount;
        return ShapeType.eCircle;
    }
};

public class PolygonShape : Shape
{
    static int MaxPolyVertexCount = 64;
    public override void Initialize()
    {
        ComputeMass(1.0f);
        body.width = width;
        body.height = width;

        init = true;
        body.SetOrigin(body.width / 2, body.height / 2);
    }

    public PolygonShape(int sizeX, int sizeY) : base("Checkers.png")
    {
        SetBox(sizeX / 2, sizeY / 2);
        width = sizeX;
        height = sizeY;
        u = new Mat2(0);

    }
    private int sizeX, sizeY;

    public int m_vertexCount;
    public Vec2[] m_vertices = new Vec2[MaxPolyVertexCount];
    public Vec2[] m_normals = new Vec2[MaxPolyVertexCount];

    public override Shape Clone()
    {
        PolygonShape poly = new PolygonShape(sizeX, sizeY);
        poly.u = u;
        for (int i = 0; i < m_vertexCount; ++i)
        {
            poly.m_vertices[i] = m_vertices[i];
            poly.m_normals[i] = m_normals[i];
        }
        poly.m_vertexCount = m_vertexCount;
        return poly;
    }

    public override void ComputeMass(float density)
    {
        // Calculate centroid and moment of interia
        Vec2 c = new Vec2(0.0f, 0.0f); // centroid
        float area = 0.0f;
        float I = 0.0f;
        const float k_inv3 = 1.0f / 3.0f;

        for (int i1 = 0; i1 < m_vertexCount; ++i1)
        {
            // Triangle vertices, third vertex implied as (0, 0)
            Vec2 p1 = m_vertices[i1];
            int i2 = i1 + 1 < m_vertexCount ? i1 + 1 : 0;
            Vec2 p2 = m_vertices[i2];

            float D = Vec2.Cross(p1, p2);
            float triangleArea = 0.5f * D;

            area += triangleArea;

            // Use area to weight the centroid average, not just vertex position
            c += triangleArea * k_inv3 * (p1 + p2);

            float intx2 = p1.x * p1.x + p2.x * p1.x + p2.x * p2.x;
            float inty2 = p1.y * p1.y + p2.y * p1.y + p2.y * p2.y;
            I += (0.25f * k_inv3 * D) * (intx2 + inty2);
        }

        c *= 1.0f / area;

        // Translate vertices to centroid (make the centroid (0, 0)
        // for the polygon in model space)
        // Not floatly necessary, but I like doing this anyway
        for (int i = 0; i < m_vertexCount; ++i)
            m_vertices[i] -= c;

        body.m = density * area;
        body.im = (body.m != 0f) ? 1.0f / body.m : 0.0f;
        body.I = I * density;
        body.iI = (body.I != 0f) ? 1.0f / body.I : 0.0f;
    }

    public override void SetOrient(float radians)
    {
        u.Set(radians);
    }

    public override void Draw()
    {
        //Console.WriteLine("Draw");
        //body.rotation = (m_vertices[1] - m_vertices[2]).angleInDeg;
        body.rotation = u.AxisY().angleInDeg;
    }

    public override ShapeType GetShapeType()
    {
        if (!init)
            return ShapeType.eCount;
        return ShapeType.ePoly;
    }

    // Half width and half height
    void SetBox(float hw, float hh)
    {
        m_vertexCount = 4;
        m_vertices[0] = new Vec2(-hw, -hh);
        m_vertices[1] = new Vec2(hw, -hh);
        m_vertices[2] = new Vec2(hw, hh);
        m_vertices[3] = new Vec2(-hw, hh);
        m_normals[0] = new Vec2(0.0f, -1.0f);
        m_normals[1] = new Vec2(1.0f, 0.0f);
        m_normals[2] = new Vec2(0.0f, 1.0f);
        m_normals[3] = new Vec2(-1.0f, 0.0f);
    }

    void Set(Vec2[] vertices, int count)
    {
        // No hulls with less than 3 vertices (ensure actual polygon)
        Debug.Assert(count > 2 && count <= MaxPolyVertexCount);
        count = Mathf.Min((int)count, MaxPolyVertexCount);

        // Find the right most point on the hull
        int rightMost = 0;
        float highestXCoord = vertices[0].x;
        for (int i = 1; i < count; ++i)
        {
            float x = vertices[i].x;
            if (x > highestXCoord)
            {
                highestXCoord = x;
                rightMost = i;
            }

            // If matching x then take farthest negative y
            else if (x == highestXCoord)
                if (vertices[i].y < vertices[rightMost].y)
                    rightMost = i;
        }

        int[] hull = new int[MaxPolyVertexCount];
        int outCount = 0;
        int indexHull = rightMost;

        for (; ; )
        {
            hull[outCount] = indexHull;

            // Search for next index that wraps around the hull
            // by computing cross products to find the most counter-clockwise
            // vertex in the set, given the previos hull index
            int nextHullIndex = 0;
            for (int i = 1; i < (int)count; ++i)
            {
                // Skip if same coordinate as we need three unique
                // points in the set to perform a cross product
                if (nextHullIndex == indexHull)
                {
                    nextHullIndex = i;
                    continue;
                }

                // Cross every set of three unique vertices
                // Record each counter clockwise third vertex and add
                // to the output hull
                // See : http://www.oocities.org/pcgpe/math2d.html
                Vec2 e1 = vertices[nextHullIndex] - vertices[hull[outCount]];
                Vec2 e2 = vertices[i] - vertices[hull[outCount]];
                float c = Vec2.Cross(e1, e2);
                if (c < 0.0f)
                    nextHullIndex = i;

                // Cross product is zero then e vectors are on same line
                // therefor want to record vertex farthest along that line
                if (c == 0.0f && e2.lengthSquared > e1.lengthSquared)
                    nextHullIndex = i;
            }

            ++outCount;
            indexHull = nextHullIndex;

            // Conclude algorithm upon wrap-around
            if (nextHullIndex == rightMost)
            {
                m_vertexCount = outCount;
                break;
            }
        }

        // Copy vertices into shape's vertices
        for (int i = 0; i < m_vertexCount; ++i)
            m_vertices[i] = vertices[hull[i]];

        // Compute face normals
        for (int i1 = 0; i1 < m_vertexCount; ++i1)
        {
            int i2 = i1 + 1 < m_vertexCount ? i1 + 1 : 0;
            Vec2 face = m_vertices[i2] - m_vertices[i1];

            // Ensure no zero-length edges, because that's bad
            Debug.Assert(face.lengthSquared < 0.000001f * 0.000001f);

            // Calculate normal with 2D cross product between vector and scalar
            m_normals[i1] = new Vec2(face.y, -face.x);
            m_normals[i1].Normalize();
        }
    }

    // The extreme point along a direction within a polygon
    public Vec2 GetSupport(Vec2 dir)
    {
        float bestProjection = -float.MaxValue;
        Vec2 bestVertex = Vec2.Zero;

        for (int i = 0; i < m_vertexCount; ++i)
        {
            Vec2 v = m_vertices[i];
            float projection = Vec2.Dot(v, dir);

            if (projection > bestProjection)
            {
                bestVertex = v;
                bestProjection = projection;
            }
        }

        return bestVertex;
    }


}


