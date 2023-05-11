using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Scenes
{
    internal class Slider : EasyDraw
    {

        Vec2 center;
        Vec2 size;
        Vec2 handleSize;

        int action;
        float reach;

        float currentValue;

        float handleScale = 1f;
        float smoothing = 0.2f;

        SceneManager sceneManager;

        EasyDraw handle;

        bool dragging;

        public Slider(Vec2 center, Vec2 size, Vec2 handleSize, Color color, Color handleColour, int action, float reach, float startValue, SceneManager parent) : base((int)size.x, (int)size.y)
        {
            this.center = center;
            this.size = size;

            SetOrigin(width / 2, height / 2);
            SetXY((int)center.x, (int)center.y);

            Clear(color);
            Fill(Color.Black);
            TextAlign(CenterMode.Center, CenterMode.Center);

            handle = new EasyDraw((int)handleSize.x, (int)handleSize.y);
            Clear(color);
            Fill(Color.Black);
            TextAlign(CenterMode.Center, CenterMode.Center);

            this.action = action;
            this.reach = reach;

            sceneManager = parent;
        }



    }
}
