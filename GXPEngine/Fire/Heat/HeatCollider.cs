using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Fire
{
    public class HeatCollider
    {

        GameObject owner;
        public HeatComponent heat;

        Vec2 size;
        Vec2 center;

        Vec2 max;
        Vec2 min;

        public HeatCollider(GameObject owner, HeatComponent heatComponent, params string[] args) : base(owner, args)
        {
            heat = heatComponent;

            max = center + size;
            min = center - size;
        }

        public void Collide(List<HeatCollider> colliders)
        {
            for(int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i].max.x <= min.x && colliders[i].min.x <= max.x &&
                    colliders[i].max.y <= min.y && colliders[i].min.y <= max.y)
                {
                    float otherHeat = colliders[i].heat.currentHeat;
                    //replace with reference to HeatComponent
                    if (otherHeat < heat.currentHeat)
                    {
                        float deltaHeat = heat.currentHeat - otherHeat;
                        otherHeat += 0.1f * deltaHeat;
                        heat.currentHeat -= 0.05f * deltaHeat;
                    }
                }
            }
        }
    }
}
