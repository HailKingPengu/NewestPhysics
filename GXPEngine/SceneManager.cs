using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Scenes;

namespace GXPEngine
{
    internal class SceneManager : GameObject
    {
        List<Scene> scenes;
        public OverlayScene overlayScene;
        int currentScene;
        bool paused;

        public SceneManager()
        {
            scenes = new List<Scene>();

            new SceneBuilder(this, scenes, new Vec2(game.width, game.height));

            AddChild(scenes[currentScene]);
        }

        public void ChangeScene(int toScene)
        {
            if (overlayScene.isActive)
            {
                RemoveChild(overlayScene);
                overlayScene.isActive = false;
                scenes[1].gameInstance.paused = false;
            }

            RemoveChild(scenes[currentScene]);
            scenes[currentScene].isActive = false;

            currentScene = toScene;

            AddChild(scenes[currentScene]);
            scenes[currentScene].isActive = true;
        }

        public void Update()
        {
            if (Input.GetKeyDown(Key.TAB) && currentScene == 1)
            {
                OverlayState();
            }
        }

        public void OverlayState()
        {
            if (overlayScene.isActive == false)
            {
                AddChild(overlayScene);
                overlayScene.isActive = true;
                scenes[1].gameInstance.paused = true;
            }
            else
            {
                RemoveChild(overlayScene);
                overlayScene.isActive = false;
                scenes[1].gameInstance.paused = false;
            }
        }
    }
}
