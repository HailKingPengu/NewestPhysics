using GXPEngine.GameInst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Fire.Editor
{
    public class EditorLevel
    {
        public void Update()
        {

        }

        public static void LoadLevel(string fileName, GameInstance instance)
        {
            var level = Serializer.ReadObject<Level>(fileName);
            foreach(LevelObject obj in level.objects)
            {
                if(obj is PhysicsObject physicsObject)
                {
                    if(physicsObject.radius != 0)
                    {
                        var a = instance.physicsWorld.CreateCircleWorldSpace(physicsObject.position, physicsObject.radius);
                        if (physicsObject.isStatic)
                        {
                            instance.AddChild(instance.physicsWorld.CreateStaticBody(physicsObject.position, physicsObject.radians, a));
                        }
                        else
                        {
                            instance.AddChild(instance.physicsWorld.CreateDynamicBody(physicsObject.position, physicsObject.radians, a));
                            
                        }
                    }
                    else
                    {
                        var a = instance.physicsWorld.CreatePolygonWorldSpace(physicsObject.vertices);
                        if (physicsObject.isStatic)
                        {
                            instance.AddChild(instance.physicsWorld.CreateStaticBody(physicsObject.position, physicsObject.radians, a));
                        }
                        else
                        {
                            instance.AddChild(instance.physicsWorld.CreateDynamicBody(physicsObject.position, physicsObject.radians, a));
                            instance.AddHeatComponentPolygon(a, false);
                        }
                    }

                }
            }
        }

    }
}
