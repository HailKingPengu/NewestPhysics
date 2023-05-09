using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions

public class MyGame : Game
{
    EasyDraw canvas;

    public MyGame() : base(800, 600, false)
    {     // Create a window that's 800x600 and NOT fullscreen


        AddChild(new AABB(0.1f, 10, new Vec2(100, 90), new Vec2(10, 10)));
        AddChild(new AABB(0.2f, 20, new Vec2(100, 150), new Vec2(20,20)));

        // Add the canvas to the engine to display it:
        //AddChild(canvas);
        Console.WriteLine("MyGame initialized");
    }

    void Update()
    {
        //canvas.Clear(Color.Black);

    }

    static void Main()
    {

        new MyGame().Start();
    }
}
