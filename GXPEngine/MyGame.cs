using System;                                   // System contains a lot of default C# libraries 
using GXPEngine;                                // GXPEngine contains the engine
using System.Drawing;                           // System.Drawing contains drawing tools such as Color definitions
using System.Collections.Generic;
using GXPEngine.Scenes;

public class MyGame : Game
{

    SceneManager sceneManager;



    public MyGame() : base(1920, 1080, true)
    {

        sceneManager = new SceneManager();
        AddChild(sceneManager);


    }

    void Update()
    {

    }

    static void Main()
    {
        new MyGame().Start();
    }
}
