using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Scenes
{
    internal class OverlayScene : Scene
    {
        //scene stuff, you know

        public OverlayScene()
        {

        }

        void Update()
        {
            if (isActive)
            {
                foreach (Button button in buttons)
                {
                    button.DoMouseCheck(Input.mouseX, Input.mouseY);
                }
            }
        }
    }
}
