using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volatile;

namespace GXPEngine.GameInst.Heat
{
    internal class ButtonController : GameObject
    {

        VoltBody vBody;
        MovingDoorController door;

        public ButtonController(VoltBody body, MovingDoorController door) 
        {
            this.vBody = body;
            this.door = door;
        }

        void Update()
        {
            if (vBody.Collision(out VoltBody body))
            {
                door.MoveDoor();
            }
        }
    }
}
