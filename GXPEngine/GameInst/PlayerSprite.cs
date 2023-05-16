using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volatile;

namespace GXPEngine.GameInst
{
    internal class PlayerSprite : AnimationSprite
    {

        VoltBody pos;

        PlayerController playerController;

        public PlayerSprite(string image, VoltBody pos) : base(image, 6, 5)
        {
            SetOrigin(width / 2, height / 2 + 15);

            width = 180;
            height = 180;

            this.pos = pos;

            //SetCycle(4, 4, 8);
        }

        void Update()
        {
            if(playerController == null)
            {
                foreach(GameObject obj in pos.parent.GetChildren())
                {
                    if(obj is PlayerController controller)
                    {
                        playerController = controller;
                    }
                }
            }

            if (pos.LinearVelocity.x > 1 || pos.LinearVelocity.x < -1)
            {
                SetCycle(0, 18, 5);
            }
            else 
            {
                SetCycle(18, 11, 5); 
            }


            Mirror(pos.LinearVelocity.x < 0, false);
            AnimateFixed();
            SetXY(pos.x, pos.y);
        }
    }
}
