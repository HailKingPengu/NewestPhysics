using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.GameInst;

namespace GXPEngine.Scenes
{
    internal class Scene : GameObject
    {
        //scene stuff, you know



        public List<Button> buttons;
        public Sprite bgImage;

        public bool isActive;

        public GameInstance gameInstance;

        public Scene()
        {
            buttons = new List<Button>();
        }

        public void AddButton(Button button)
        {
            buttons.Add(button);
            AddChild(button);
        }

        public void AddBackground(Sprite image, Vec2 screenSize)
        {
            bgImage = image;
            bgImage.width = (int)screenSize.x;
            bgImage.height = (int)screenSize.y;
            AddChildAt(bgImage, 0);
        }

        public void AddGame(GameInstance gameInstance)
        {
            this.gameInstance = gameInstance;
            AddChildAt(this.gameInstance, 1000);
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
