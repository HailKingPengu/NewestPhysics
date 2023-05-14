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

        public static Level LoadLevel(string fileName)
        {
            var level = Serializer.ReadObject<Level>(fileName);
            return level;
        }

    }
}
