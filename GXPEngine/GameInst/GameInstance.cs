using GXPEngine.Fire;
using GXPEngine.Fire.Editor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volatile;

namespace GXPEngine.GameInst
{
    internal class GameInstance : Pivot
    {
        int initTicks = 10;
        public bool paused;


        //public EasyDraw debugger;


        VoltWorld physicsWorld;

        List<Pivot> layers;

        VoltPolygon e;

        VoltPolygon[] AAAs;

        VoltCircle player;
        PlayerController playerController;
        PlayerSprite playerSprite;

        public List<HeatCollider> heatColliders;

        float camSmoothing = 0.1f;

        //Environmental_Sound_System sound;

        Level level;


        #region Editor Vars
        bool isEditor = true;
        bool poly = true;
        int clicks = 0;
        Vec2[] points = new Vec2[4];
        Vec2 min;
        Vec2 max;
        Vec2 center;

        #endregion



        //layer 0 = foreground
        //layer 1 = physics/player area
        //layer 2+ = background

        public GameInstance() 
        {
            if (isEditor)
            {

                level = new Level();

                physicsWorld = new VoltWorld();
                return;
            }

            sound = new Environmental_Sound_System();

            sound.StartMusic();


            heatColliders = new List<HeatCollider>();


            AAAs = new VoltPolygon[30];

            physicsWorld = new VoltWorld();
            AAAs = new VoltPolygon[5];

            var a = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-25, -25), new Vec2(-25, 25), new Vec2(25, 25), new Vec2(25, -25) });
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 200), 0, new VoltShape[] { a }));

            List<HeatComponent> components = new List<HeatComponent>();

            for (int i = 0; i < 30; i++)
            {
                var AAA = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-15, -15), new Vec2(-15, 15), new Vec2(15, 15), new Vec2(15, -15) });
                AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 200), 0, new VoltShape[] { AAA }));


                AAAs[i] = AAA;

                components.Add(new HeatComponent(AAAs[i].Body, 2, false));
                AAAs[i].AddChild(components[i]);
                heatColliders.Add(components[i].returnCollider());
            }

            var BBB = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-10, -10), new Vec2(-10, 10), new Vec2(10, 10), new Vec2(10, -10) });
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 200), 0, new VoltShape[] { BBB }));
            HeatComponent BBBHeat = new HeatComponent(BBB.Body, 2, false);
            BBB.AddChild(BBBHeat);
            heatColliders.Add(BBBHeat.returnCollider());

            var CCC = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-5, -5), new Vec2(-5, 5), new Vec2(5, 5), new Vec2(5, -5) });
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 200), 0, new VoltShape[] { CCC }));
            HeatComponent CCCHeat = new HeatComponent(CCC.Body, 2, false);
            CCC.AddChild(CCCHeat);
            heatColliders.Add(CCCHeat.returnCollider());

            var c = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-25, -25), new Vec2(-25, 25), new Vec2(25, 25), new Vec2(25, -25) });
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 250), 0, new VoltShape[] { c }));
            HeatComponent cHeat = new HeatComponent(c.Body, 2, false);
            c.AddChild(cHeat);
            heatColliders.Add(cHeat.returnCollider());

            var d = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-25, -25), new Vec2(-25, 25), new Vec2(25, 25), new Vec2(25, -25) });
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 300), 0, new VoltShape[] { d }));
            HeatComponent dHeat = new HeatComponent(d.Body, 2, false);
            d.AddChild(dHeat);
            heatColliders.Add(dHeat.returnCollider());


            e = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-25, -25), new Vec2(-25, 25), new Vec2(25, 25), new Vec2(25, -25) });
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(800, 100), 0, new VoltShape[] { e }));

            var b = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-800, -20), new Vec2(-800, 20), new Vec2(800, 20), new Vec2(800, -20) });
            AddChild(physicsWorld.CreateStaticBody(new Vec2(600, 500), 0, new VoltShape[] { b }));

            var f = physicsWorld.CreateCircleWorldSpace(new Vec2(600, 200), 20);
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 200), 0, new VoltShape[] { f }));

            player = physicsWorld.CreateCircleWorldSpace(new Vec2(500, 200), 20, 0.01f, 0.95f, 0);
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(500, 200), 0, new VoltShape[] { player }));

            playerController = new PlayerController(physicsWorld, this);
            player.AddChild(playerController);
            playerController.Set();

            playerSprite = new PlayerSprite("../../Assets/doof.png");
            playerController.AddChild(playerSprite);


            //debugger = new EasyDraw(1200, 900);
            //AddChildAt(debugger, 100);


            //physicsScene = new PhysicsScene(10);
            //var b = physicsScene.Add(new PolygonShape(50, 50), 210, 200);
            //AddChild(b);
            ////b.SetStatic();

            //var a = physicsScene.Add(new PolygonShape(50, 50), 200, 500);

            //AddChild(a);

            //a.SetStatic();

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



            //Serializer.WriteToBinaryFile("Level1.dat", level);
        }

        void Update()
        {
            if (isEditor)
            {
                if (Input.GetKeyDown(Key.S))
                {
                    PhysicsObject obj;
                    foreach (VoltBody body in GetChildren())
                    {
                        obj = new PhysicsObject();
                        if (body.shapes[0] is VoltCircle circle)
                        {
                            obj.radians = body.Angle;
                            obj.radius = circle.radius;
                            obj.position = body.Position;
                        }
                        else if (body.shapes[0] is VoltPolygon poly)
                        {
                            obj.position = body.Position;
                            obj.vertices = poly.bodyVertices;
                            obj.radians = body.Angle;
                        }
                        level.objects.Add(obj);
                    }

                    Serializer.WriteObject("level.dat", level);
                    
                }

                if(Input.GetMouseButtonDown(0))
                {

                    if (poly)
                    {
                        switch (clicks)
                        {
                            case 0:
                                {
                                    points = new Vec2[4];
                                    points[0] = new Vec2(Input.mouseX, Input.mouseY);
                                    clicks++;
                                    break;
                                }
                            case 1:
                                {
                                    points[2] = new Vec2(Input.mouseX, Input.mouseY);
                                    min.x = Mathf.Min(points[0].x, points[2].x);
                                    min.y = Mathf.Min(points[0].y, points[2].y);
                                    max.x = Mathf.Max(points[0].x, points[2].x);
                                    max.y = Mathf.Max(points[0].y, points[2].y);
                                    points[0] = new Vec2(min.x, min.y);
                                    points[1] = new Vec2(min.x, max.y);
                                    points[2] = new Vec2(max.x, max.y);
                                    points[3] = new Vec2(max.x, min.y);
                                    center = new Vec2(min.x + (max.x - min.x)/2, min.y +  (max.y - min.y)/2);
                                    var a = physicsWorld.CreatePolygonWorldSpace(points);
                                    AddChild(physicsWorld.CreateDynamicBody(center, 0, a));
                                    clicks = 0;
                                    break;
                                }
                        }
                            

                    }
                }
                physicsWorld.RunUpdate();
                return;
            }
            //Console.WriteLine("FUCK");
            //if (initTicks > 0)
            //{
            //    initTicks -= 1;
            //    return;
            //}


            if (!paused)
            {


                foreach(HeatCollider heatCol in heatColliders)
                {
                    heatCol.CalculateCurrent();
                }
                foreach (HeatCollider heatCol in heatColliders)
                {
                    heatCol.Collide(heatColliders);
                }

                //if (Input.GetMouseButtonDown(0))
                //{

                //    //Vec2 playerPos = new Vec2(player.Body.x, player.Body.y);
                //    //Vec2 mousePos = new Vec2(Input.mouseX, Input.mouseY);



                //    //e.Body.AddForce(new Vec2(-10000, 0));

                //    //foreach(VoltPolygon POLY in AAAs)
                //    //{
                //    //    POLY.Body.AddForce(new Vec2(Utils.Random(-2000, 2000), Utils.Random(-2000, 2000)));
                //    //}

                //    //physicsWorld.Bodies.First().AddForce(new Vec2(0, -1000));
                //}
            }
                


            if(Time.deltaTime > 50 )
            {
                Console.WriteLine("Lag");
            }

            //physicsScene.Step();
            //if (!paused)
            //{
            //    player.UpdateGeneral();
            //    player.UpdateInput(87, 65, 83, 68, 69, 81);
            //}

            float camPosition = Mathf.Clamp(-player.Body.x + 600, -2000, 400);

            //Console.WriteLine(camPosition);

            x += (camPosition - x) * camSmoothing; 

        }
    }
}
