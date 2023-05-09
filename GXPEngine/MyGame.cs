using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions

public class Boids : Game
{
    EasyDraw canvas;

    public Boids() : base(1920, 1080, true)
    {     // Create a window that's 800x600 and NOT fullscreen

        // Draw some things on a canvas:
        canvas = new EasyDraw(1920, 1080);
        canvas.Clear(Color.MediumPurple);
        canvas.Fill(Color.Yellow);
        canvas.Ellipse(width / 2, height / 2, 200, 200);
        canvas.Fill(50);
        canvas.TextSize(32);
        canvas.TextAlign(CenterMode.Center, CenterMode.Center);
        canvas.Text("Welcome!", width / 2, height / 2);

        // Add the canvas to the engine to display it:
        AddChild(canvas);
        Console.WriteLine("MyGame initialized");
    }

    // For every game object, Update is called every frame, by the engine:
    void Update()
    {
        canvas.Clear(Color.Black);

    }

    static void Main()                          // Main() is the first method that's called when the program is run
    {
        new Boids().Start();
    }
}
