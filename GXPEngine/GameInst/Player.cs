using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.GameInst
{
    internal class Player : AnimationSprite
    {
        public EasyDraw groundCheck;
        public EasyDraw ceilingCheck;

        public float velocityX;
        public float velocityY;

        float airFriction;
        float groundFriction;

        float gravity = 0.03f;
        float movementForce = 0.08f;
        float jumpForce = 10f;

        int direction;

        public bool grounded = false;

        public Player(string image, float airFriction, float groundFriction) : base(image, 4, 6)
        {
            SetOrigin(width / 2, height / 2);
            this.airFriction = airFriction;
            this.groundFriction = groundFriction;

            groundCheck = new EasyDraw(width - 1, 10);
            AddChild(groundCheck);
            groundCheck.SetOrigin(groundCheck.width / 2, groundCheck.height / 2);
            groundCheck.SetXY(0, height / 2);

            ceilingCheck = new EasyDraw(width - 1, 10);
            AddChild(ceilingCheck);
            ceilingCheck.SetOrigin(ceilingCheck.width / 2, ceilingCheck.height / 2);
            ceilingCheck.SetXY(0, -height / 2);

            SetCycle(4, 4, 8);
        }

        public void setMovementValues(float gravity, float movementForce, float jumpForce)
        {
            this.gravity = gravity;
            this.movementForce = movementForce;
            this.jumpForce = jumpForce;
        }

        public void UpdateGeneral(float camY)
        {

            if (groundCheck.GetCollisions(false).Length > 1 && groundCheck.GetCollisions(false)[0].y != 0)
            {
                grounded = true;

                y = groundCheck.GetCollisions(false)[0].y - groundCheck.y;
                //y = groundCheck.GetCollisions(false)[0].y - groundCheck.y - (Convert.ToInt32(camY / tileSize) * tileSize) - tileSize / 2;
            }
            else
            {
                grounded = false;
            }

            //Console.WriteLine(grounded);

            if (ceilingCheck.GetCollisions(false).Length > 1)
            {
                y += ceilingCheck.height / 2;
                y += velocityY;
                velocityY = 0;
            }



            velocityX /= groundFriction;
            velocityY /= airFriction;

            x += velocityX;
            y += velocityY;

            if (GetCollisions(false).Length > 2)
            {
                x -= velocityX;
            }
        }

        public void UpdateInput(int up, int left, int down, int right, int dig, int bomb)
        {

            bool isMoving = false;

            if (!grounded)
            {
                velocityY += gravity * Time.deltaTime;
            }
            else
            {
                velocityY = 0;
            }

            if (Input.GetKey(right))
            {
                velocityX += movementForce * Time.deltaTime;
                direction = 1;
                SetCycle(0 + 12 * direction, 4);
                isMoving = true;

            }

            //d
            if (Input.GetKey(left))
            {
                velocityX -= movementForce * Time.deltaTime;
                direction = 0;
                SetCycle(0 + 12 * direction, 4);
                isMoving = true;
            }

            //w
            if (Input.GetKeyDown(up) && grounded)
            {
                y -= 10;
                velocityY -= jumpForce;
                grounded = false;
            }

            if (!isMoving)
            {
                SetCycle(4 + 12 * direction, 4);
            }


            AnimateFixed();
        }

    }
}
