using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volatile;

namespace GXPEngine.Fire
{
    public class HeatCollider : EasyDraw
    {

        public VoltBody owner;
        public HeatComponent heat;

        Vec2 size;
        Vec2 center;

        Vec2 max;
        Vec2 min;

        Vec2 currentMax;
        Vec2 currentMin;

        float extraY;

        public HeatCollider(VoltBody owner, HeatComponent heatComponent, params string[] args) : base(100, 100)
        {
            this.owner = owner;
            heat = heatComponent;

            //max = center + size;
            //min = center - size;

            //max = new Vec2(owner.x, owner.y) + 100 / owner.scale;
            //min = new Vec2(owner.x, owner.y) - 100 / owner.scale;

            SetOrigin(width/2, height/2);
            Clear(255, 100, 0, 100);

            TextAlign(CenterMode.Min, CenterMode.Min);

            max = new Vec2(50 * owner.shapes[0].scaleX, 50 * owner.shapes[0].scaleY);
            min = new Vec2(-50 * owner.shapes[0].scaleX, -50 * owner.shapes[0].scaleY);
        }

        //extraY = -0.02 (x-4.15)^(4)+6;

        public void CalculateCurrent()
        {





            //Console.WriteLine(max + " - " + min);

            currentMax = max + new Vec2(owner.x, owner.y);
            currentMin = min + new Vec2(owner.x, owner.y);
        }

        public void Collide(List<HeatCollider> colliders)
        {

            if (heat.burning)
            {
                Clear(255, 0, 50, 150);
            }

            for(int i = colliders.IndexOf(this) + 1; i < colliders.Count; i++)
            {
                if (colliders[i] != null || colliders[i] != this)
                {

                    //if (colliders[i].owner.AABB.Left - 20 <= owner.AABB.Right + 20 && colliders[i].owner.AABB.Right + 20 >= owner.AABB.Left - 20 &&
                    //    colliders[i].owner.AABB.Top - 20 <= owner.AABB.Bottom + 20 && colliders[i].owner.AABB.Bottom + 20 <= owner.AABB.Top - 20)


                    if (colliders[i].currentMin.x <= currentMax.x && colliders[i].currentMax.x >= currentMin.x &&
                        colliders[i].currentMin.y <= currentMax.y && colliders[i].currentMax.y >= currentMin.y)
                    {
                        HeatComponent otherHeat = colliders[i].heat;
                        //replace with reference to HeatComponent
                        //if (heat.burning)
                        //{
                        //    colliders[i].heat.burning = true;
                        //}

                        if (heat.burning && otherHeat.currentHeat < heat.currentHeat && otherHeat.currentHeat <= colliders[i].heat.burnThreshold)
                        {
                            float deltaHeat = Mathf.Clamp((heat.currentHeat - otherHeat.currentHeat) / 3, 0, colliders[i].heat.burnThreshold);
                            colliders[i].heat.currentHeat += 0.01f * deltaHeat;
                            
                            //heat.currentHeat -= 0.05f * deltaHeat;
                        }
                        else if(otherHeat.burning && heat.currentHeat < otherHeat.currentHeat && heat.currentHeat <= otherHeat.burnThreshold)
                        {
                            float deltaHeat = Mathf.Clamp((otherHeat.currentHeat - heat.currentHeat) / 3, 0, heat.burnThreshold);
                            heat.currentHeat += 0.01f * deltaHeat;
                        }
                        if(heat.burning && !colliders[i].heat.burning)
                        {
                            colliders[i].heat.currentHeat += 6f;
                        }
                        else if(otherHeat.burning && !heat.burning)
                        {
                            heat.currentHeat += 6f;
                        }

                    }
                }
            }
        }

        public void Update()
        {
            if (!heat.burning)
            {
                Clear(255, 100, 0, 100);
            }
            else
            {
                Clear(255, 0, 50, 150);
            }

            Text(heat.currentHeat.ToString() + "\n" + heat.burnThreshold.ToString() + "\n" + heat.timer.ToString() + "\n" + heat.burnTime.ToString());
        }
    }
}
