using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions
using System.Collections.Generic;
using GXPEngine.Scenes;

public class MyGame : Game
{

    List<Scene> scenes;

    int currentScene = 0;



    public MyGame() : base(800, 600, false)
    {

        scenes = new List<Scene>();
        // Draw some things on a canvas:
        canvas = new EasyDraw(1920, 1080);
        canvas.Clear(Color.MediumPurple);
        //canvas.Fill(Color.Yellow);
        //canvas.Ellipse(width / 2, height / 2, 200, 200);
        //canvas.Fill(50);
        //canvas.TextSize(32);
        //canvas.TextAlign(CenterMode.Center, CenterMode.Center);
        //canvas.Text("Welcome!", width / 2, height / 2);
        AddChild(a);
        a.velocity = new Vec2(-1, 0);
        //var b = new AABB(0.2f, 20, new Vec2(200, 200), new Vec2(100, 100));
        var b = new Circle(0.2f, 20, new Vec2(100, 200), 100);
        AddChild(b);
        AddChild(canvas);
        var a = new AABB(0.1f, 10, new Vec2(400, 200), new Vec2(100, 200));

        scenes.Add(new Scene());
        scenes[0].AddButton(new Button(new Vec2(400, 300), new Vec2(200, 50), Color.White, "gaming", 0, 0));
        scenes[0].AddButton(new Button(new Vec2(150, 300), new Vec2(200, 50), Color.White, "gaming", 0, 0));
        scenes[0].AddButton(new Button(new Vec2(650, 300), new Vec2(200, 50), Color.White, "gaming", 0, 0));
        physicsObjects.Add(a);
        physicsObjects.Add(b);

        scenes[0].AddButton(new Button(new Vec2(400, 150), new Vec2(200, 50), Color.White, "gaming", 0, 0));
        scenes[0].AddButton(new Button(new Vec2(150, 150), new Vec2(200, 50), Color.White, "gaming", 0, 0));
        scenes[0].AddButton(new Button(new Vec2(650, 150), new Vec2(200, 50), Color.White, "gaming", 0, 0));

        scenes[0].AddButton(new Button(new Vec2(400, 450), new Vec2(200, 50), Color.White, "gaming", 0, 0));
        scenes[0].AddButton(new Button(new Vec2(150, 450), new Vec2(200, 50), Color.White, "gaming", 0, 0));
        scenes[0].AddButton(new Button(new Vec2(650, 450), new Vec2(200, 50), Color.White, "gaming", 0, 0));

        scenes[0].AddButton(new Button(new Vec2(400, 225), new Vec2(200, 50), Color.White, "gaming", 0, 0));
        scenes[0].AddButton(new Button(new Vec2(150, 225), new Vec2(200, 50), Color.White, "gaming", 0, 0));
        scenes[0].AddButton(new Button(new Vec2(650, 225), new Vec2(200, 50), Color.White, "gaming", 0, 0));

        scenes[0].AddButton(new Button(new Vec2(400, 375), new Vec2(200, 50), Color.White, "gaming", 0, 0));
        scenes[0].AddButton(new Button(new Vec2(150, 375), new Vec2(200, 50), Color.White, "gaming", 0, 0));
        scenes[0].AddButton(new Button(new Vec2(650, 375), new Vec2(200, 50), Color.White, "gaming", 0, 0));

        AddChild(scenes[currentScene]);


        // Add the canvas to the engine to display it:
        Console.WriteLine("MyGame initialized");
    }

    void Update()
    {
        //canvas.Clear(Color.White);

    }

    static void Main()
    {
        new MyGame().Start();
    }
}
