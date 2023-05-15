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
    public class GameInstance : Pivot
    {
        int initTicks = 10;
        public bool paused;


        //public EasyDraw debugger;

        public VoltWorld physicsWorld;

        List<Pivot> layers;

        VoltPolygon e;

        VoltPolygon[] AAAs;

        VoltCircle player;
        PlayerController playerController;
        PlayerSprite playerSprite;

        public List<HeatCollider> heatColliders;

        float camSmoothing = 0.1f;

        Environmental_Sound_System sound;

        Level level;


        #region Editor Vars

        bool isEditor = true;
        bool poly = false;
        int clicks = 0;
        Vec2[] points = new Vec2[4];
        Vec2 min;
        Vec2 max;
        Vec2 center;
        float radius;
        bool testing;
        bool isStatic = false;
        GameObject lastAdded;

        #endregion



        //layer 0 = foreground
        //layer 1 = physics/player area
        //layer 2+ = background

        public GameInstance()
        {
            physicsWorld = new VoltWorld();

            heatColliders = new List<HeatCollider>();

            if (isEditor)
            {

                level = new Level();

                return;
            }

            //sound = new Environmental_Sound_System();

            //sound.StartMusic();


            heatColliders = new List<HeatCollider>();


            AAAs = new VoltPolygon[7];

            //physicsWorld = new VoltWorld();
            //AAAs = new VoltPolygon[5];

            var a = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-25, -25), new Vec2(-25, 25), new Vec2(25, 25), new Vec2(25, -25) });
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 200), 0, new VoltShape[] { a }));

            List<HeatComponent> components = new List<HeatComponent>();

            for (int i = 0; i < 7; i++)
            {
                var AAA = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-10, -10), new Vec2(-10, 10), new Vec2(10, 10), new Vec2(10, -10) });
                AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 200), 0, new VoltShape[] { AAA }));


                AAAs[i] = AAA;

                AddHeatComponentPolygon(AAAs[i], false);
            }

            var BBB = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-10, -10), new Vec2(-10, 10), new Vec2(10, 10), new Vec2(10, -10) });
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 200), 0, new VoltShape[] { BBB }));
            AddHeatComponentPolygon(BBB, false);

            var CCC = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-5, -5), new Vec2(-5, 5), new Vec2(5, 5), new Vec2(5, -5) });
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 200), 0, new VoltShape[] { CCC }));
            AddHeatComponentPolygon(CCC, false);

            var c = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-25, -25), new Vec2(-25, 25), new Vec2(25, 25), new Vec2(25, -25) });
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 250), 0, new VoltShape[] { c }));
            AddHeatComponentPolygon(c, false);

            var d = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-25, -25), new Vec2(-25, 25), new Vec2(25, 25), new Vec2(25, -25) });
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 300), 0, new VoltShape[] { d }));
            AddHeatComponentPolygon(d, false);


            e = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-25, -25), new Vec2(-25, 25), new Vec2(25, 25), new Vec2(25, -25) });
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(800, 100), 0, new VoltShape[] { e }));

            var b = physicsWorld.CreatePolygonBodySpace(new Vec2[] { new Vec2(-800, -20), new Vec2(-800, 20), new Vec2(800, 20), new Vec2(800, -20) });
            AddChild(physicsWorld.CreateStaticBody(new Vec2(600, 500), 0, new VoltShape[] { b }));

            var f = physicsWorld.CreateCircleWorldSpace(new Vec2(600, 200), 20);
            AddChild(physicsWorld.CreateDynamicBody(new Vec2(600, 200), 0, new VoltShape[] { f }));

            player = physicsWorld.CreateCircleWorldSpace(new Vec2(500, 200), 20, 0.01f, 0.95f, 0);
            VoltBody playerBody = physicsWorld.CreateDynamicBody(new Vec2(500, 200), 0, new VoltShape[] { player });
            //playerBody. = false;
            AddChild(playerBody);

            playerController = new PlayerController(physicsWorld, this);
            player.AddChild(playerController);
            playerController.Set();

            playerSprite = new PlayerSprite("../../Assets/freg.png");
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
                if (Input.GetKeyDown(Key.LEFT_SHIFT))
                {
                    poly = !poly;
                }
                if (Input.GetKey(Key.LEFT_CTRL))
                {
                    if (Input.GetKey(Key.S))
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
                                obj.isStatic = body.IsStatic;
                            }
                            else if (body.shapes[0] is VoltPolygon poly)
                            {
                                obj.position = body.Position;
                                obj.vertices = poly.bodyVertices;
                                obj.radians = body.Angle;
                                obj.isStatic = body.IsStatic;
                            }
                            level.objects.Add(obj);
                        }

                        Serializer.WriteObject("level.dat", level);
                    }
                    if (Input.GetKey(Key.L))
                    {
                        EditorLevel.LoadLevel("level.dat", this);
                    }

                    else if (Input.GetKey(Key.Z))
                    {
                        RemoveChild(lastAdded);
                    }
                }
                if (Input.GetKeyDown(Key.S))
                {
                    isStatic = !isStatic;
                }

                if (Input.GetKey(Key.R))
                {
                    var last = (lastAdded as VoltBody);
                    last.Angle += 0.01f;
                    last.Facing = VoltMath.Polar(last.Angle);
                }

                if (Input.GetMouseButtonDown(0))
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
                                    center = new Vec2(min.x + (max.x - min.x) / 2, min.y + (max.y - min.y) / 2);
                                    var a = physicsWorld.CreatePolygonWorldSpace(points);
                                    lastAdded = isStatic ? physicsWorld.CreateStaticBody(center, 0, a) : physicsWorld.CreateDynamicBody(center, 0, a);
                                    AddChild(lastAdded);

                                    AddHeatComponentPolygon(a, false);

                                    clicks = 0;
                                    break;
                                }
                        }


                    }
                    else
                    {
                        switch (clicks)
                        {
                            case 0:
                                {
                                    center = new Vec2(Input.mouseX, Input.mouseY);
                                    clicks++;
                                    break;
                                }
                            case 1:
                                {
                                    radius = Mathf.Abs(Vec2.Distance(center, new Vec2(Input.mouseX, Input.mouseY)));
                                    var b = physicsWorld.CreateCircleWorldSpace(center, radius);
                                    lastAdded = isStatic ? physicsWorld.CreateStaticBody(center, 0, b) : physicsWorld.CreateDynamicBody(center, 0, b);
                                    AddChild(lastAdded);
                                    clicks = 0;
                                    break;
                                }
                        }
                    }
                }
                if (testing)
                {

                    physicsWorld.RunUpdate();
                }
                if (Input.GetKeyDown(Key.ENTER))
                {
                    testing = !testing;
                }
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


                foreach (HeatCollider heatCol in heatColliders)
                {
                    heatCol.CalculateCurrent();
                }


                foreach (HeatCollider heatCol in heatColliders)
                {
                    heatCol.Collide(heatColliders);
                }

                physicsWorld.RunUpdate();
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



            if (Time.deltaTime > 50)
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

        public void AddHeatComponentPolygon(VoltPolygon poly, bool startBurning)
        {
            HeatComponent heat = new HeatComponent(poly.Body, 2, startBurning);
            poly.AddChild(heat);
            heatColliders.Add(heat.returnCollider());
        }

        public void AddHeatComponentCircle(VoltCircle poly, bool startBurning)
        {
            HeatComponent heat = new HeatComponent(poly.Body, 2, startBurning);
            poly.AddChild(heat);
            heatColliders.Add(heat.returnCollider());
        }
    }
}
