using GXPEngine.GameInst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volatile;

namespace GXPEngine.Fire
{
    public class HeatComponent:Pivot
    {

        VoltBody owner;

        HeatCollider colliderChild;

        public float currentHeat;
        public float burnThreshold;
        // /\ set to private after use in debug

        public bool burning;

        public int burnTime;
        public int timer;

        //in millis
        int baseBurnTime = 5000;

        public HeatComponent(VoltBody owner, float materialThreshold, bool startBurning)
        {

            colliderChild = new HeatCollider(owner, this, new string[1]);
            AddChild(colliderChild);

            this.owner = owner;

            burnThreshold = materialThreshold * owner.shapes[0].bodySpaceAABB.Area;

            burnTime = (int)(30 * Mathf.Sqrt((materialThreshold * owner.shapes[0].bodySpaceAABB.Area) * 4)) + baseBurnTime;

            burning = startBurning;

            if (startBurning)
            {
                currentHeat = burnThreshold;
            }

            //if (owner.shapes[0].bodySpaceAABB)
            //{
            //    burnThreshold = materialThreshold * FindPolygonSurf(owner.Collider as PolygonCollider);
            //}
            //else if(owner is CircleCollider) 
            //{
            //    burnThreshold = materialThreshold * ((owner.Collider as CircleCollider).Radius * (owner.Collider as CircleCollider).Radius * Mathf.PI);
            //}

        }

        public HeatCollider returnCollider()
        {
            return colliderChild;
        }

        void Update()
        {

            rotation = -Vec2.RadToDeg(owner.Angle);

            //Console.WriteLine(Vec2.RadToDeg(owner.Angle));

            //if (burning)
            //{
            //    //list of all HeatColliders
            //    colliderChild.Collide(new List<HeatCollider>());
            //}

            if(currentHeat >= burnThreshold)
            {
                burning = true;

                timer += Time.deltaTime;
            }
            if(!burning)
            {
                currentHeat *= 0.999f;
            }

            if(timer > burnTime)
            {
                burning = false;
                LateRemove();
                LateDestroy();
            }
        }


        //float FindPolygonSurf(PolygonCollider collider) 
        //{
        //    Vec2[] points = collider.Points;

        //    float minx = float.MaxValue;
        //    float miny = float.MaxValue;
        //    float maxx = float.MinValue;
        //    float maxy = float.MinValue;h.
        //    for (int i = 0; i < points.Length; i++)
        //    {
        //        if (points[i].x < minx) minx = points[i].x;
        //        if (points[i].y < miny) miny = points[i].y;
        //        if (points[i].x > maxx) maxx = points[i].x;
        //        if (points[i].y > maxy) maxy = points[i].y;
        //    }

        //    return (maxx - minx) * (maxy - maxx);

        //}


    }
}
