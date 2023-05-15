using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volatile;

namespace GXPEngine.GameInst.Heat
{
    internal class MovingDoorController : GameObject
    {

        VoltBody door;

        Vec2 moveBy;
        int duration;

        int movedBy;

        bool doorMoving = false;

        public MovingDoorController(VoltBody door, Vec2 moveBy, int duration)
        {
            this.door = door;
            this.moveBy = moveBy;
            this.duration = duration;

        }

        public void MoveDoor()
        {
            doorMoving = true;
        }

        void Update()
        {
            if (doorMoving)
            {
                //door.LateDestroy();
                //door.LateRemove();

                if(movedBy < duration)
                {
                    VoltBody oldDoor = door;

                    (parent.parent.parent as GameInstance).physicsWorld.RemoveBody(door);

                    (parent.parent.parent as GameInstance).physicsWorld.AddBody(oldDoor, new Vec2(oldDoor.x, oldDoor.y) + (moveBy / duration) * Time.deltaTime, 0);


                    //door.x += (moveBy.x / duration) * Time.deltaTime;
                    //door.y += (moveBy.y / duration) * Time.deltaTime;

                    movedBy += Time.deltaTime;
                }
                else
                {
                    doorMoving = false;
                }
            }
        }
    }
}
