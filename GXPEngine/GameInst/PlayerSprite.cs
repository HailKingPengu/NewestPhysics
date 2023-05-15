using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.GameInst
{
    internal class PlayerSprite : AnimationSprite
    {

        public PlayerSprite(string image) : base(image, 1, 1)
        {
            SetOrigin(width / 2, height / 2 + 15);

            width = 120;
            height = 120;

            SetCycle(4, 4, 8);
        }
    }
}
