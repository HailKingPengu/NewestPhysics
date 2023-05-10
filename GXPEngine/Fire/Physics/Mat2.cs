using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Mat2
{
    public float m00, m01;
    public float m10, m11;


    public float[,] m;
    public float[] v;
    public Mat2(float radians)
    {
        m = new float[2, 2];
        v = new float[4];
        float c = Mathf.Cos(radians);
        float s = Mathf.Sin(radians);

        m00 = c; m01 = -s;
        m10 = s; m11 = c;
    }

    public  Mat2(float a, float b, float c, float d)
    {
        m00 = a;
        m01 = b;
        m10 = c;
        m11 = d;
    }

    public void Set(float radians)
    {
        float c = Mathf.Cos(radians);
        float s = Mathf.Sin(radians);

        m00 = c; m01 = -s;
        m10 = s; m11 = c;
    }

    public Mat2 Abs()
    {
        return new Mat2(Mathf.Abs(m00), Mathf.Abs(m01), Mathf.Abs(m10), Mathf.Abs(m11));
    }

    public Vec2 AxisX()
    {
        return new Vec2(m00, m10);
    }

    public Vec2 AxisY()
    {
        return new Vec2(m01, m11);
    }

    public Mat2 Transpose()
    {
        return new Mat2(m00, m10, m01, m11);
    }

    public static Vec2 operator *(Vec2 rhs, Mat2 m)
    {
        return new Vec2(m.m00 * rhs.x + m.m01 * rhs.y, m.m10 * rhs.x + m.m11 * rhs.y);
    }

    public static Vec2 operator * ( Mat2 m, Vec2 rhs)
    {
        return new Vec2(m.m00 * rhs.x + m.m01 * rhs.y, m.m10 * rhs.x + m.m11 * rhs.y);
    }

    public static Mat2 operator *(Mat2 rhs, Mat2 m)
    {
        // [00 01]  [00 01]
        // [10 11]  [10 11]

        return new Mat2(
          m.m[0, 0] * rhs.m[0, 0] + m.m[0, 1] * rhs.m[1, 0],
          m.m[0, 0] * rhs.m[0, 1] + m.m[0, 1] * rhs.m[1, 1],
          m.m[1, 0] * rhs.m[0, 0] + m.m[1, 1] * rhs.m[1, 0],
          m.m[1, 0] * rhs.m[0, 1] + m.m[1, 1] * rhs.m[1, 1]
        );
    }
};
