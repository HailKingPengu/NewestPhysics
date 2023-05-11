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
            this.handleSize = handleSize;

            SetOrigin(width / 2, height / 2);
            SetXY((int)center.x, (int)center.y);
            Clear(color);

            handle = new EasyDraw((int)handleSize.x, (int)handleSize.y);
            handle.SetOrigin(handle.width / 2, handle.height / 2);
            handle.SetXY(-(width / 2) + width * (startValue / reach), 0);
            handle.Clear(handleColour);
            AddChild(handle);

            this.action = action;
            this.reach = reach;

            sceneManager = parent;
        }

        public void DoMouseCheck(int mouseX, int mouseY)
        {
            if (mouseX <= center.x + (size.x / 2) && mouseX >= center.x - (size.x / 2)
            &&  mouseY <= center.y + (handleSize.y / 2 * handle.scale) && mouseY >= center.y - (handleSize.y / 2 * handle.scale))
            {
                handleScale = 1.3f;
                if (Input.GetMouseButtonDown(0))
                {
                    dragging = true;
                    Slide();
                }
            }
            else
            {
                handleScale = 1f;

            }

            if(dragging && Input.GetMouseButton(0))
            {
                Slide();
            }
            else
            {
                dragging = false;
            }
        }

        void Update()
        {
            handle.scale -= (handle.scale - handleScale) * smoothing;
        }

        void Slide()
        {
            float clampedMouseX = Mathf.Clamp(Input.mouseX, center.x - size.x / 2, center.x + size.x / 2);
            handle.x = clampedMouseX - center.x;
            currentValue = ((clampedMouseX - (center.x - size.x / 2)) / size.x) * reach;
        }
    }
}
