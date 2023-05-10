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

        public Button(Vec2 center, Vec2 size, Color color, string text, int action, int result) : base((int)size.x, (int)size.y)
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
        }

        public void DoMouseCheck(int mouseX, int mouseY)
        {
            if(mouseX <= center.x + size.x / 2 && mouseX >= center.x - size.x / 2 
            && mouseY <= center.y + size.y / 2 && mouseY >= center.y - size.y / 2)
            {
                desiredScale = 1.5f;
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
    }
}
