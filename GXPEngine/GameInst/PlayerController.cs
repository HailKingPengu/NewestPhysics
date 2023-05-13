using GXPEngine.Fire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using TiledMapParser;
using Volatile;

namespace GXPEngine.GameInst
{
    internal class PlayerController : Pivot
    {

        VoltWorld physicsWorld;
        GameInstance gameInstance;

        VoltCircle playerCollider;
        VoltBody playerBody;

        AimIndicator aimIndicator;

        bool aiming = false;

        float offsetX;

        public PlayerController(VoltWorld physicsWorld, GameInstance gameInstance)
        {

            this.physicsWorld = physicsWorld;
            this.gameInstance = gameInstance;

        }

        public void Set()
        {
            playerCollider = parent as VoltCircle;
            playerBody = playerCollider.Body;

            this.aimIndicator = new AimIndicator(playerBody);
            AddChild(aimIndicator);
        }

        void Update()
        {
            if (!gameInstance.paused)
            {

                offsetX = gameInstance.x;

                if (aiming)
                {
                    aimIndicator.UpdateAiming(offsetX);
                }

                Vec2 playerPos = new Vec2(playerBody.x, playerBody.y);
                Vec2 mousePos = new Vec2(Input.mouseX - offsetX, Input.mouseY);

                rotation = -parent.rotation;


                if (Input.GetMouseButtonUp(0) &&
                    Input.mouseX - offsetX < playerBody.x + 32 && Input.mouseX - offsetX > playerBody.x - 32 &&
                    Input.mouseY < playerBody.y + 32 && Input.mouseY > playerBody.y - 32)
                {
                    if (!aiming)
                    {
                        aiming = true;
                        aimIndicator.StartAiming();
                    }
                    else
                    {
                        aiming = false;
                        aimIndicator.StopAiming();
                    }
                }

                if (Input.GetMouseButtonDown(0) && aiming)
                {
                    Shoot(playerPos, mousePos);
                }

                if (Input.GetMouseButton(0) && !aiming)
                {
                    if (new Vec2(playerBody.LinearVelocity.x, 0).length < 30)
                        playerBody.AddForce(new Vec2(Mathf.Clamp((mousePos.x - playerPos.x) / 50, -1, 1), 0));
                }
                else
                {
                    playerBody.LinearVelocity *= new Vec2(0.8f, 1);
                }
            }
        }

        void Shoot(Vec2 playerPos, Vec2 mousePos)
        {
            //gameInstance.debugger.ClearTransparent();
            //gameInstance.debugger.Fill(0);
            //gameInstance.debugger.Stroke(0);
            //gameInstance.debugger.Rect(playerPos.x, playerPos.y, 3,3);

            //gameInstance.debugger.Rect(mousePos.x, mousePos.y, 3, 3);

            Vec2 aim = (mousePos - playerPos).normalized;

            var fireball = physicsWorld.CreateCircleWorldSpace(playerPos + aim * 35, 10);
            gameInstance.AddChild(physicsWorld.CreateDynamicBody(playerPos + aim * 35, 0, new VoltShape[] { fireball }));
            fireball.AddChild(new HeatComponent(fireball.Body, 2));

            Vec2 force = ((mousePos - playerPos) * 8f);

            if (force.length < 1500)
            {
                force *= 1500 / force.length;
            }
            else if (force.length > 2500)
            {
                force *= 2500 / force.length;
            }
            fireball.Body.AddForce(force);

            aiming = false;
            aimIndicator.StopAiming();
        }
    }
}
