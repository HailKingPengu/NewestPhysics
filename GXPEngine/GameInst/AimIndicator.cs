using GXPEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volatile;

namespace GXPEngine.GameInst
{
    internal class AimIndicator : EasyDraw
    {

        VoltBody playerBody;

        Vec2[] steps;
        int stepNum = 12;
        float stepLength = 80;
        float addedY = 5f;

        public AimIndicator(VoltBody playerBody) : base(1200, 1200)
        {
            SetOrigin(width / 2, height / 2);
            scale = 0.4333f;

            this.playerBody = playerBody;

            StrokeWeight(3);

            steps = new Vec2[stepNum];

            visible = false;
        }

        public void UpdateAiming(float offsetX)
        {
            ClearTransparent();

            //Vec2 relMousePoint = new Vec2(width / 2 - playerBody.x + Input.mouseX, height / 2 - playerBody.y + Input.mouseY).normalized * 200;

            Vec2 playerPos = new Vec2(playerBody.x, playerBody.y);
            Vec2 mousePos = new Vec2(Input.mouseX - offsetX, Input.mouseY);

            //Line(width / 2, height / 2, relMousePoint.x, relMousePoint.y);


            Vec2 force = ((mousePos - playerPos) * 8f);

            if (force.length < 1500)
            {
                force *= 1500 / force.length;
            }
            else if (force.length > 2500)
            {
                force *= 2500 / force.length;
            }


            stepLength = force.length * 0.05f;

            steps[0] = new Vec2(Input.mouseX - offsetX - playerBody.x, Input.mouseY - playerBody.y).normalized * stepLength;

            Ellipse((width / scale) / 2 + steps[0].x, (height / scale) / 2 + steps[0].y, 10, 10);

            for (int i = 1; i < steps.Length; i++)
            {
                steps[i] = steps[i-1] + (steps[0] * 0.71f) + i * new Vec2(0, addedY);

                Ellipse((width / scale) / 2 + steps[i].x, (height / scale) / 2 + steps[i].y, 10 - (int)(0.8 * i), 10 - (int)(0.8 * i));
            }
        }

        public void StartAiming()
        {
            visible = true;
        }

        public void StopAiming()
        {
            visible = false;
        }
    }
}
