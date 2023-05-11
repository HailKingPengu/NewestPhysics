using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.GameInst;

namespace GXPEngine.Scenes
{
    internal class SceneBuilder
    {
        public SceneBuilder(SceneManager sceneManager, List<Scene> scenes, Vec2 screenSize) 
        {
            //main menu
            scenes.Add(new Scene());
            scenes[0].AddButton(new Button(new Vec2(400, 300), new Vec2(200, 50), Color.White, "gaming", 0, 1, sceneManager));
            scenes[0].AddButton(new Button(new Vec2(400, 380), new Vec2(200, 50), Color.White, "settings", 0, 2, sceneManager));
            scenes[0].AddButton(new Button(new Vec2(400, 460), new Vec2(200, 50), Color.White, "touch grass", 1, 0, sceneManager));
            scenes[0].AddBackground(new Sprite("../../Assets/Background1.png"), screenSize);
            scenes[0].isActive = true;

            //game
            scenes.Add(new Scene());
            //scenes[1].AddButton(new Button(new Vec2(400, 300), new Vec2(200, 50), Color.White, "WAAAAA", 0, 0, sceneManager));
            scenes[1].AddBackground(new Sprite("../../Assets/Background2.jpg"), screenSize);
            scenes[1].AddGame(new GameInstance());
            scenes[1].isActive = false;

            //settings
            scenes.Add(new Scene());
            scenes[2].AddButton(new Button(new Vec2(400, 300), new Vec2(200, 50), Color.White, "back", 0, 0, sceneManager));
            scenes[2].AddButton(new Button(new Vec2(400, 380), new Vec2(200, 50), Color.White, "sound idk", 0, 2, sceneManager));
            scenes[2].AddButton(new Button(new Vec2(400, 460), new Vec2(200, 50), Color.White, "bomb the nurburgring", 0, 2, sceneManager));
            scenes[2].AddBackground(new Sprite("../../Assets/Background3.gif"), screenSize);
            scenes[2].isActive = false;

            OverlayScene overlayScene = new OverlayScene();
            overlayScene.AddButton(new Button(new Vec2(400, 300), new Vec2(200, 50), Color.White, "resume", 2, 0, sceneManager));
            overlayScene.AddButton(new Button(new Vec2(400, 380), new Vec2(200, 50), Color.White, "settings", 0, 3, sceneManager));
            overlayScene.AddButton(new Button(new Vec2(400, 460), new Vec2(200, 50), Color.White, "exit to main menu", 0, 0, sceneManager));
            overlayScene.AddBackground(new Sprite("../../Assets/OverlayBackground.png"), screenSize);
            overlayScene.isActive = false;
            sceneManager.overlayScene = overlayScene;

            //overlaysettings
            scenes.Add(new Scene());
            scenes[3].AddButton(new Button(new Vec2(400, 300), new Vec2(200, 50), Color.White, "back", 0, 1, sceneManager));
            scenes[3].AddButton(new Button(new Vec2(400, 380), new Vec2(200, 50), Color.White, "sound idk", 0, 3, sceneManager));
            scenes[3].AddButton(new Button(new Vec2(400, 460), new Vec2(200, 50), Color.White, "bomb the nurburgring", 0, 3, sceneManager));
            scenes[3].AddBackground(new Sprite("../../Assets/OverlayBackground.png"), screenSize);
            scenes[3].isActive = false;
        }

    }
}
