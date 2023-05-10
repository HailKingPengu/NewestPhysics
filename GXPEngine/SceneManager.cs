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
        int currentScene;
        bool paused;

        public SceneManager()
        {
            scenes = new List<Scene>();

            SceneBuilder sb = new SceneBuilder(this, scenes, new Vec2(game.width, game.height));

            AddChild(scenes[currentScene]);
        }

        public void ChangeScene(int toScene)
        {
            RemoveChild(scenes[currentScene]);
            scenes[currentScene].isActive = false;

            currentScene = toScene;

            AddChild(scenes[currentScene]);
            scenes[currentScene].isActive = true;
        }
    }
}
