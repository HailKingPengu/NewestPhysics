using GXPEngine.Managers;
using System;
using System.Collections.Generic;
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
            //check if has collided?
            //if(fireball)
        }

    }
}
