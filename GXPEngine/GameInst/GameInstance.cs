using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.GameInst
{
    internal class GameInstance : Pivot
    {
        int initTicks = 10;
        bool paused;

        PhysicsScene physicsScene;

        Player player;

        List<Pivot> layers;

        //layer 0 = foreground
        //layer 1 = physics/player area
        //layer 2+ = background

        public GameInstance() 
        {

            physicsScene = new PhysicsScene(10);
            var b = physicsScene.Add(new PolygonShape(50, 50), 230, 200);
            AddChild(b);
            //b.SetStatic();

            var a = physicsScene.Add(new PolygonShape(50, 50), 200, 500);

            AddChild(a);

            a.SetStatic();

            //var c = physicsScene.Add(new Circle(50), 230, 200);
            //AddChild(c);
            ////b.SetStatic();

            //var d = physicsScene.Add(new Circle(50), 200, 500);

            //AddChild(d);

            //d.SetStatic();

            //EasyDraw testcube = new EasyDraw(1000, 200);
            //testcube.SetXY(0, 600);
            //testcube.Clear(205);
            //AddChild(testcube);

            //player = new Player("doof.png", 1.02f, 1.1f);
            //AddChild(player);
        }

        void Update()
        {
            if (initTicks > 0)
            {
                initTicks -= 1;
                return;
            }
            physicsScene.Step();
            //if (!paused)
            //{
            //    player.UpdateGeneral();
            //    player.UpdateInput(87, 65, 83, 68, 69, 81);
            //}
        }
    }
}
