using GXPEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace A {
    public class PhysicsManager
    {
        List<CollisionData> collisions = new List<CollisionData>();

        List<Pair> pairs = new List<Pair>();

        public void Step()
        {
            float updatesPerSecond = 60;
            float timeBetweenUpdates = 1 / updatesPerSecond;
            float timeTakenForLastFrame = Time.deltaTime / 1000f;
            if (timeTakenForLastFrame > 0.2f)
                timeTakenForLastFrame = 0.2f;
            //while(timeTakenForLastFrame > timeBetweenUpdates)
            //{
            //    timeTakenForLastFrame -= timeBetweenUpdates;
            //}

            collisions.Clear();
            //CheckAllCollisions();
            ResolveCollisions();


        }

        //private void CheckAllCollisions()
        //{
        //    for (int i = 0; i < Game.main.physicsObjects.Count(); i++)
        //    {
        //        int next = i + 1;
        //        if (next >= Game.main.physicsObjects.Count())
        //            next = 0;
        //        Check(Game.main.physicsObjects[i], Game.main.physicsObjects[next]);
        //    }
        //}

        void ResolveCollision(CollisionData data)
        {
            Console.WriteLine("RESOLVE");
            // Calculate relative velocity 
            Vec2 rv = data.b.velocity - data.a.velocity;
            // Calculate relative velocity in terms of the normal direction 
            float velAlongNormal = Vec2.Dot(rv, data.normal);
            // Do not resolve if velocities are separating 
            if (velAlongNormal > 0)
                return;
            // Calculate restitution 
            float e = Mathf.Min(data.a.material.restitution, data.b.material.restitution);
            // Calculate impulse scalar 
            float j = -(1 + e) * velAlongNormal;
            j /= data.a.massData.inverseMass + data.b.massData.inverseMass;
            // Apply impulse 
            Vec2 impulse = j * data.normal;
            data.a.velocity -= data.a.massData.inverseMass * impulse;
            data.b.velocity += data.b.massData.inverseMass * impulse;
        }

        void ResolveCollisions()
        {
            CollisionData earliest;
            foreach (CollisionData data in collisions)
            {
                ResolveCollision(data);
            }
        }

        void Check(PhysicsObject a, PhysicsObject b)
        {
            CollisionData data = new CollisionData();
            CollisionData collision = new CollisionData();
            collision.isEmpty = true;
            if (a is Circle CCa && b is Circle CCb)
            {
                if (CirclevsCircle(CCa, CCb, out data))
                {
                    PositionalCorrection(data);
                    collision = data;
                    collision.isEmpty = false;
                }
            }
            else if (a is AABB AAa && b is AABB AAb)
            {
                if (AABBvsAABB(AAa, AAb, out data))
                {
                    PositionalCorrection(data);
                    collision = data;
                    collision.isEmpty = false;
                }
            }
            else if (a is AABB ACa && b is Circle ACb)
            {
                if (AABBvsCircle(ACa, ACb, out data))
                {
                    PositionalCorrection(data);
                    collision = data;
                    collision.isEmpty = false;
                }
            }
            else if (a is Circle CAa && b is AABB CAb)
            {
                if (AABBvsCircle(CAb, CAa, out data))
                {
                    PositionalCorrection(data);
                    collision = data;
                    collision.isEmpty = false;
                }
            }
            else
            {

            }
            if (!collision.isEmpty) collisions.Add(collision);

        }

        bool CirclevsCircle(Circle A, Circle B, out CollisionData data)
        {
            data.a = A;
            data.b = B;
            data = new CollisionData();
            // Vector from A to B 
            Vec2 n = B.position - A.position;
            float r = A.radius + B.radius;
            r *= r;
            if (n.lengthSquared > r)
                return false;
            // Circles have collided, now compute manifold 
            float d = n.length; // perform actual sqrt 
                                // If distance between circles is not zero 
            if (d != 0)
            {
                // Distance is difference between radius and distance 
                data.penetration = r - d;
                // Utilize our d since we performed sqrt on it already within Length( ) 
                // Points from A to B, and is a unit vector 
                data.normal = n / d;
                return true;
            }
            // Circles are on same position 
            else
            {
                // Choose random (but consistent) values 
                data.penetration = A.radius;
                data.normal = new Vec2(1, 0);
                return true;
            }
        }

        bool AABBvsAABB(AABB A, AABB B, out CollisionData data)
        {
            data = new CollisionData();
            data.a = A;
            data.b = B;
            // Vector from A to B 
            Vec2 n = B.position - A.position;

            // Calculate half extents along x axis for each object 
            float a_extent = (A.max.x - A.min.x) / 2;
            float b_extent = (B.max.x - B.min.x) / 2;

            // Calculate overlap on x axis 
            float x_overlap = a_extent + b_extent - Mathf.Abs(n.x);

            // SAT test on x axis 
            if (x_overlap > 0)
            {
                // Calculate half extents along x axis for each object 
                a_extent = (A.max.y - A.min.y) / 2;
                b_extent = (B.max.y - B.min.y) / 2;

                // Calculate overlap on y axis 
                float y_overlap = a_extent + b_extent - Mathf.Abs(n.y);

                // SAT test on y axis 
                if (y_overlap > 0)
                {
                    // Find out which axis is axis of least penetration 
                    if (x_overlap < y_overlap)
                    {
                        // Point towards B knowing that n points from A to B 
                        if (n.x < 0)
                            data.normal = new Vec2(-1, 0);
                        else
                            data.normal = new Vec2(0, 0);
                        data.penetration = x_overlap;
                        return true;
                    }
                    else
                    {
                        // Point toward B knowing that n points from A to B 
                        if (n.y < 0)
                            data.normal = new Vec2(0, -1);
                        else
                            data.normal = new Vec2(0, 1);
                        data.penetration = y_overlap;
                        return true;
                    }
                }
            }
            return false;
        }

        bool AABBvsCircle(AABB A, Circle B, out CollisionData data)
        {
            data = new CollisionData();
            data.a = A;
            data.b = B;
            // Vector from A to B 
            Vec2 n = B.position - A.position;
            // Closest point on A to center of B 
            Vec2 closest = n;
            // Calculate half extents along each axis 
            float x_extent = (A.max.x - A.min.x) / 2;
            float y_extent = (A.max.y - A.min.y) / 2;
            // Clamp point to edges of the AABB 
            closest.x = Mathf.Clamp(-x_extent, x_extent, closest.x);
            closest.y = Mathf.Clamp(-y_extent, y_extent, closest.y);
            bool inside = false;
            // Circle is inside the AABB, so we need to clamp the circle's center 
            // to the closest edge 
            if (n == closest)
            {
                inside = true;
                // Find closest axis 
                if (Mathf.Abs(n.x) > Mathf.Abs(n.y))
                {
                    // Clamp to closest extent 
                    if (closest.x > 0)
                        closest.x = x_extent;
                    else
                        closest.x = -x_extent;
                }
                // y axis is shorter 
                else
                {
                    // Clamp to closest extent 
                    if (closest.y > 0)
                        closest.y = y_extent;
                    else
                        closest.y = -y_extent;
                }
            }
            Vec2 normal = n - closest;
            float d = normal.lengthSquared;
            float r = B.radius;
            // Early out of the radius is shorter than distance to closest point and 
            // Circle not inside the AABB 
            if (d > r * r && !inside)
                return false;
            // Avoided sqrt until we needed 
            d = Mathf.Sqrt(d);
            // Collision normal needs to be flipped to point outside if circle was 
            // inside the AABB 
            if (inside)
            {
                data.normal = Vec2.Zero - n;
                data.penetration = r - d;
            }
            else
            {
                data.normal = n;
                data.penetration = r - d;
            }
            return true;
        }

        void PositionalCorrection(CollisionData data)
        {
            const float percent = 0.2f; // usually 20% to 80% 
            const float slop = 0.01f; // usually 0.01 to 0.1 
            Vec2 correction = (Mathf.Max(data.penetration - slop, 0.0f) / (data.a.massData.inverseMass + data.b.massData.inverseMass) * percent * data.normal);
            data.a.position -= data.a.massData.inverseMass * correction;
            data.b.position += data.b.massData.inverseMass * correction;
        }

        void BroadPhase()
        {
            pairs.Clear();

            BoundingBox A;
            BoundingBox B;
        }
    }
}
