﻿using GXPEngine.Fire;
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
    internal class FireballDestroyer : GameObject
    {

        VoltBody fireball;

        //destroys fireballs when they hit an object
        public FireballDestroyer(VoltBody fireball) 
        {
            this.fireball = fireball;
        }

        public void Update()
        {
            if (fireball.Collision(out VoltBody body))
            {
                List<GameObject> children = body.shapes[0].GetChildren();
                foreach (GameObject child in children) 
                {
                    if(child is HeatComponent heat)
                    {
                        HeatComponent otherHeat = child as HeatComponent;
                        otherHeat.currentHeat = otherHeat.burnThreshold;
                        otherHeat.burning = true;
                    }
                }
                (parent.parent.parent as GameInstance).physicsWorld.RemoveBody(fireball);
                fireball.Remove();
                fireball.LateDestroy();
                LateDestroy();
            }
        }
    }
}
