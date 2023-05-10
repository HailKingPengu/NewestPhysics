using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.GameInst
{
    internal class GameInstance : Pivot
    {

        bool paused;

        Player player;

        List<Pivot> layers;

        //layer 0 = foreground
        //layer 1 = physics/player area
        //layer 2+ = background

        public GameInstance() 
        {





            //EasyDraw testcube = new EasyDraw(1000, 200);
            //testcube.SetXY(0, 600);
            //testcube.Clear(205);
            //AddChild(testcube);

            //player = new Player("doof.png", 1.02f, 1.1f);
            //AddChild(player);
        }

        void Update()
        {
            //if (!paused)
            //{
            //    player.UpdateGeneral();
            //    player.UpdateInput(87, 65, 83, 68, 69, 81);
            //}
        }
    }
}
