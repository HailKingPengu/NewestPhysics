using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Scenes
{
    internal class Scene : GameObject
    {
        //scene stuff, you know

        public List<Button> buttons;

        public Scene()
        {
            buttons = new List<Button>();
        }

        public void AddButton(Button button)
        {
            buttons.Add(button);
            AddChild(button);
        }

        void Update()
        {
            foreach(Button button in buttons)
            {
                button.DoMouseCheck(Input.mouseX, Input.mouseY);
            }
        }
    }
}
