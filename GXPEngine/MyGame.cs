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

        scenes.Add(new Scene());
        scenes[0].AddButton(new Button(new Vec2(400, 300), new Vec2(200, 50), Color.White, "gaming", 0, 0));
        scenes[0].AddButton(new Button(new Vec2(150, 300), new Vec2(200, 50), Color.White, "gaming", 0, 0));
        scenes[0].AddButton(new Button(new Vec2(650, 300), new Vec2(200, 50), Color.White, "gaming", 0, 0));

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


    }

    void Update()
    {

    }

    static void Main()
    {
        new MyGame().Start();
    }
}
