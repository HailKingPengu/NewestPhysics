using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine.GameInst
{
    internal class GameInstance : Pivot
    {

        bool paused;

        Player player;

        List<Pivot> layers;

        public GameInstance() 
        { 
            
        }
    }
}
