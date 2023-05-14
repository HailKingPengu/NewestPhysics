using GXPEngine.Fire;
using GXPEngine.Managers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volatile;

namespace GXPEngine.GameInst.Heat
{
    internal class FireballDestroyer
    {

        VoltBody fireball;

        //destroys fireballs when they hit an object
        public FireballDestroyer(VoltBody fireball) 
        {
            this.fireball = fireball;
        }

        public void update()
        {
            if (fireball.Collision(out VoltBody body))
            {
                List<GameObject> children = body.shapes[0].GetChildren();
                foreach (GameObject child in children) 
                {
                    if(child is HeatComponent heat)
                    {
                       
                    }
                }
            }
            //check if has collided?
            //if(fireball)
        }

    }
}
