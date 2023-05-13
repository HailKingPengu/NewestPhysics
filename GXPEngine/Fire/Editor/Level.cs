using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.Fire.Editor
{
    [DataContract]
    public class Level
    {
        [DataMember]
        public List<LevelObject> objects;

        public Level()
        {
            objects = new List<LevelObject>();
        }

    }
}
