using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions

public class Boids : Game
{
    EasyDraw canvas;

    int numBoids = 250;

    Boid[] boids;

    float boidSpeed = 3;
    float[] boidX;
    float[] boidY;
    float[] boidRot;
    float[] boidMoveX;
    float[] boidMoveY;

    int screenSizeX = 1920;
    int screenSizeY = 1080;

    float distThresholdAlign = 10;
    float distThresholdSepar = 10;
    float distThresholdCohes = 15;
    float dist;
    float closestDist;
    int closestI;
    int[] isClose;

    int friends;

    float normFac;

    float closeBoidMoveX;
    float closeBoidMoveY;
    float closeBoidMoveXNorm;
    float closeBoidMoveYNorm;

    float difX;
    float difY;
    float difXNorm;
    float difYNorm;

    float closeBoidX;
    float closeBoidY;
    float closeBoidXNorm;
    float closeBoidYNorm;

    //control values
    float alignment = 0.015f;
    float separation = 0.008f;
    float cohesion = 0.0003f;

    float correctionForce = 0.3f;


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

        boids = new Boid[numBoids];

        for (int i = 0; i < numBoids; i++)
        {
            boids[i] = new Boid(numBoids, distThresholdSepar, separation, distThresholdAlign, alignment, distThresholdCohes, cohesion, i, screenSizeX, screenSizeY);
        }

        // Add the canvas to the engine to display it:
        AddChild(canvas);
        Console.WriteLine("MyGame initialized");
    }

    // For every game object, Update is called every frame, by the engine:
    void Update()
    {

        canvas.Clear(Color.Black);

        for (int i = 0; i < numBoids; i++)
        {
            float[] boidCoords = boids[i].updateBoid(boids);
            canvas.Ellipse(boidCoords[0],boidCoords[1],5,5);

        }
    }

    static void Main()                          // Main() is the first method that's called when the program is run
    {
        new Boids().Start();
    }
}
