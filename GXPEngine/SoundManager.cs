using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    internal class SoundManager
    {

        Sound music;
        SoundChannel musicChannel;

        public SoundManager() 
        {
            music = new Sound("../../Assets/songPlaceholder.mp3");
            musicChannel = music.Play();
        }

        static void MusicVolume(float volume)
        {
            musicChannel.Volume = volume / 100;
        }
    }
}
