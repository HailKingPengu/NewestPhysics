using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game
{
    EasyDraw canvas;

    public MyGame() : base(800, 600, false)
    {     // Create a window that's 800x600 and NOT fullscreen

        // Draw some things on a canvas:
        canvas = new EasyDraw(1920, 1080);
        canvas.Clear(Color.MediumPurple);
        //canvas.Fill(Color.Yellow);
        //canvas.Ellipse(width / 2, height / 2, 200, 200);
        //canvas.Fill(50);
        //canvas.TextSize(32);
        //canvas.TextAlign(CenterMode.Center, CenterMode.Center);
        //canvas.Text("Welcome!", width / 2, height / 2);
        AddChild(canvas);
        var a = new AABB(0.1f, 10, new Vec2(400, 200), new Vec2(100, 200));
        AddChild(a);
        a.velocity = new Vec2(-1, 0);
        //var b = new AABB(0.2f, 20, new Vec2(200, 200), new Vec2(100, 100));
        var b = new Circle(0.2f, 20, new Vec2(100, 200), 100);
        AddChild(b);

        physicsObjects.Add(a);
        physicsObjects.Add(b);

        // Add the canvas to the engine to display it:
        Console.WriteLine("MyGame initialized");
    }

    // For every game object, Update is called every frame, by the engine:
    void Update()
    {
        //canvas.Clear(Color.White);

    }

    static void Main()                          // Main() is the first method that's called when the program is run
    {

        new MyGame().Start();
    }
}
