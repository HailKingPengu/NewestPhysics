using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Scenes
{
    internal class Button : EasyDraw
    {
        Vec2 center;
        Vec2 size;

        int action;
        int result;

        float desiredScale = 1f;
        float smoothing = 0.2f;

        SceneManager sceneManager;

        //bool mouseHover;

        public Button(Vec2 center, Vec2 size, Color color, string text, int action, int result, SceneManager parent) : base((int)size.x, (int)size.y)
        { 
            this.center = center;
            this.size = size;

            SetOrigin(width/2, height/2);
            SetXY((int)center.x, (int)center.y);

            Clear(color);
            Fill(Color.Black);
            TextAlign(CenterMode.Center, CenterMode.Center);
            Text(text);

            this.action = action;
            this.result = result;

            sceneManager = parent;
        }

        public void DoMouseCheck(int mouseX, int mouseY)
        {
            if (mouseX <= center.x + (size.x / 2 * scale) && mouseX >= center.x - (size.x / 2 * scale)
            &&  mouseY <= center.y + (size.y / 2 * scale) && mouseY >= center.y - (size.y / 2 * scale))
            {
                desiredScale = 1.3f;
                if (Input.GetMouseButtonUp(0))
                {
                    Action();
                }
            }
            else
            {
                desiredScale = 1f;

            }
        }

        void Update()
        {
            scale -= (scale - desiredScale) * smoothing;
        }

        void Action()
        {
            switch (action)
            {
                case (0):
                    sceneManager.ChangeScene(result);
                break;
                case (1):
                    System.Environment.Exit(0);
                break;
            }
        }
    }
}
