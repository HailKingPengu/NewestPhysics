using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    public class Boid
    {
        float boidSpeed = 3;
        float boidX;
        float boidY;
        float boidRot;
        float boidMoveX;
        float boidMoveY;

        int isClose;

        int numBoids;
        float distThresholdSepar;
        float distThresholdAlign;
        float distThresholdCohes;

        int id;

        float separation;
        float alignment;
        float cohesion;

        int screenSizeX;
        int screenSizeY;

        float normFac;

        public Boid(int numBoids, float distThresholdSepar, float separation, float distThresholdAlign, float alignment, float distThresholdCohes, float cohesion, int id, int screenSizeX, int screenSizeY)
        {
            boidX = Utils.Random(0, 1920);
            boidY = Utils.Random(0, 1080);

            boidMoveX = Utils.Random(-1, 1);
            boidMoveY = Utils.Random(-1, 1);

            this.numBoids = numBoids;
            this.distThresholdSepar = distThresholdSepar;
            this.distThresholdAlign = distThresholdAlign;
            this.distThresholdCohes = distThresholdCohes;
            this.id = id;

            this.separation = separation;
            this.alignment = alignment;
            this.cohesion = cohesion;           

            this.screenSizeX = screenSizeX; 
            this.screenSizeY = screenSizeY;
        }

        public float[] updateBoid(Boid[] boids)
        {

            int friends = 0;

            //base values to prevent invalid values
            float closestDist = 10000;
            float closeBoidMoveX = 0.000001f;
            float closeBoidMoveY = 0.000001f;
            float closeBoidX = 0.000001f;
            float closeBoidY = 0.000001f;

            int closestI = 0;

            //forloop for every other boid in existence (i2)
            for (int i = 0; i < numBoids; i++)
            {
                float dist = Mathf.Sqrt(Mathf.Abs(boidX - boids[i].boidX) + Mathf.Abs(boidY - boids[i].boidY));

                //find closest boid
                if (dist < closestDist && dist > 0.1 && dist < distThresholdSepar)
                {
                    closestDist = dist;
                    closestI = i;
                }
                //find all within threshold and calculate average movement
                if (dist < distThresholdAlign && dist > 0.1)
                {
                    friends++;

                    closeBoidMoveX += boids[i].boidMoveX;
                    closeBoidMoveY += boids[i].boidMoveY;
                }
                //find all within threshold and calculate average position
                if (dist < distThresholdCohes && dist > 0.1)
                {
                    closeBoidX += boids[i].boidX - boidX;
                    closeBoidY += boids[i].boidY - boidY;
                }
            }

            if (closestI != id)
            {
                //all rules

                //normalize average movement boids within threshold
                //ALIGNMENT
                normFac = 1 / Mathf.Sqrt(Mathf.Abs(closeBoidMoveX) + Mathf.Abs(closeBoidMoveY));
                float closeBoidMoveXNorm = closeBoidMoveX * normFac * alignment;
                float closeBoidMoveYNorm = closeBoidMoveY * normFac * alignment;

                boidMoveX = boidMoveX + closeBoidMoveXNorm;
                boidMoveY = boidMoveY + closeBoidMoveYNorm;

                //normalize movement away from closest boid
                //SEPARATION
                float difX = boids[closestI].boidX - boidX;
                float difY = boids[closestI].boidY - boidY;
                normFac = 1 / Mathf.Sqrt(Mathf.Abs(difX) + Mathf.Abs(difY));
                float difXNorm = difX * normFac * -separation;
                float difYNorm = difY * normFac * -separation;

                boidMoveX = boidMoveX + difXNorm;
                boidMoveY = boidMoveY + difYNorm;

                //normalize positions of boids within threshold
                //COHESION
                normFac = 1 / Mathf.Sqrt(Mathf.Abs(closeBoidX) + Mathf.Abs(closeBoidY));
                float closeBoidXNorm = closeBoidX * normFac * cohesion;
                float closeBoidYNorm = closeBoidY * normFac * cohesion;

                boidMoveX = boidMoveX + closeBoidXNorm;
                boidMoveY = boidMoveY + closeBoidYNorm;
            }

            //normalize movement and move boid
            normFac = 1 / (Mathf.Sqrt(Mathf.Abs(boidMoveX) + Mathf.Abs(boidMoveY)));
            boidMoveX = boidMoveX * normFac;
            boidMoveY = boidMoveY * normFac;

            boidX += boidMoveX * boidSpeed;
            boidY += boidMoveY * boidSpeed;

            /*
            //reset when hit side
             if (boidX[i] > screenSizeX)
             {
             boidX[i] -= screenSizeX;
             }
             if (boidY[i] > screenSizeY)
             {
             boidY[i] -= screenSizeY;
             }
             */
            if (boidX < 0)
            {
                boidX += screenSizeX;
            }
            if (boidY < 0)
            {
                boidY += screenSizeY;
            }

            boidX = boidX % screenSizeX;
            boidY = boidY % screenSizeY;

            float[] position = new float[] { boidX, boidY };

            return position;

            //canvas.Ellipse(boidX, boidY, 5, 5);

            //draw boid and its direction vector
            /*stroke(380 - (friends - 1) * 12.5, 255 - (friends - 1) * 25, (friends - 1) * 25);
            fill(380 - (friends - 1) * 12.5, 255 - (friends - 1) * 25, (friends - 1) * 25);
            strokeWeight(2);

            rectMode(CENTER);
            line(boidX[i], boidY[i], boidX[i] + boidMoveX[i] * 12, boidY[i] + boidMoveY[i] * 12);
            rect(boidX[i], boidY[i], 10, 10);*/

        }

    }
}
