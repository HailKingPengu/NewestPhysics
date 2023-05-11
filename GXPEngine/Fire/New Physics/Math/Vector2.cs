/*
 *  VolatilePhysics - A 2D Physics Library for Networked Games
 *  Copyright (c) 2015-2016 - Alexander Shoulson - http://ashoulson.com
 *
 *  This software is provided 'as-is', without any express or implied
 *  warranty. In no event will the authors be held liable for any damages
 *  arising from the use of this software.
 *  Permission is granted to anyone to use this software for any purpose,
 *  including commercial applications, and to alter it and redistribute it
 *  freely, subject to the following restrictions:
 *  
 *  1. The origin of this software must not be misrepresented; you must not
 *     claim that you wrote the original software. If you use this software
 *     in a product, an acknowledgment in the product documentation would be
 *     appreciated but is not required.
 *  2. Altered source versions must be plainly marked as such, and must not be
 *     misrepresented as being the original software.
 *  3. This notice may not be removed or altered from any source distribution.
*/

#if !UNITY
using GXPEngine;

namespace Volatile1
{
  public struct Vec2
  {
    public static Vec2 zero { get { return new Vec2(0.0f, 0.0f); } }

    public static float Dot(Vec2 a, Vec2 b)
    {
      return (a.x * b.x) + (a.y * b.y);
    }

    public readonly float x;
    public readonly float y;

    public float lengthSquared
    {
      get
      {
        return (this.x * this.x) + (this.y * this.y);
      }
    }

    public float magnitude 
    { 
      get 
      {
        return Mathf.Sqrt(this.lengthSquared);
      } 
    }

    public Vec2 normalized
    {
      get
      {
        float magnitude = this.magnitude;
        return new Vec2(this.x / magnitude, this.y / magnitude);
      }
    }

    public Vec2 (float x, float y)
    {
      this.x = x;
      this.y = y;
    }

    public static Vec2 operator *(Vec2 a, float b)
    {
      return new Vec2(a.x * b, a.y * b);
    }

    public static Vec2 operator *(float a, Vec2 b)
    {
      return new Vec2(b.x * a, b.y * a);
    }

    public static Vec2 operator +(Vec2 a, Vec2 b)
    {
      return new Vec2(a.x + b.x, a.y + b.y);
    }

    public static Vec2 operator -(Vec2 a, Vec2 b)
    {
      return new Vec2(a.x - b.x, a.y - b.y);
    }

    public static Vec2 operator -(Vec2 a)
    {
      return new Vec2(-a.x, -a.y);
    }
  }
}
#endif