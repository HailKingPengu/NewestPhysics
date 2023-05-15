using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


[DataContract]
public class PhysicsObject : LevelObject
{
    [DataMember]
    public Vec2 position;
    [DataMember]
    public float radians;
    [DataMember]
    public Vec2[] vertices;
    [DataMember]
    public float radius;
    [DataMember]
    public bool isStatic;
}

