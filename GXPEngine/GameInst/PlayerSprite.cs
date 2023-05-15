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

        public PlayerSprite(string image, VoltBody pos) : base(image, 1, 1)
        {
            SetOrigin(width / 2, height / 2 + 15);

            width = 180;
            height = 180;

            this.pos = pos;

            SetCycle(4, 4, 8);
        }

        void Update()
        {
            SetXY(pos.x, pos.y);
        }
    }
}
