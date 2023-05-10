using GXPEngine;
using GXPEngine.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace A
{
    public class PhysicsObject : Sprite
    {
        public EasyDraw draw;
        public Vec2 position
        {
            get
            {
                return new Vec2(x, y);
            }
            set
            {
                x = value.x;
                y = value.y;
            }
        }
        public Vec2 velocity;
        public Vec2 Force;
        public Material material;
        public MassData massData;

        public PhysicsObject(Material material, float mass, Vec2 pos, string fileName) : base(fileName)
        {
            this.massData = new MassData();
            this.material = material;
            position = pos;
            massData.mass = mass;
            if (massData.mass == 0)
            {
                massData.inverseMass = 0;
            }
            else
            {
                massData.inverseMass = 1 / mass;
            }
        }

        public void Update()
        {
            Draw();
            Move();
        }

        void Move()
        {
            velocity += (massData.inverseMass * Force) * (Time.deltaTime / 1000f);
            position += velocity * (Time.deltaTime / 1000f);
        }

        public virtual void Draw()
        {

        }

        public virtual void GetBoundingBox(out BoundingBox box)
        {
            box = new BoundingBox();
        }


    }
}
