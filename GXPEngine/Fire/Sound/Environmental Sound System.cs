using GXPEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Environmental_Sound_System : GameObject
{
    Sound music;
    SoundChannel musicChannel;

    public List<Sound> ambientSounds;

    public Environmental_Sound_System()
    {
        music = new Sound("../../Assets/songPlaceholder.mp3");
        foreach(string fileName in Directory.GetFiles("../../Assets/AmbientSounds/"))
        {
            if(File.Exists(fileName))
            {
                ambientSounds.Add(new Sound(fileName));
            }
        }
    }


    public void StartMusic()
    {
        musicChannel = music.Play();

    }

    void PlayRandomSound()
    {
        var a = new Random().Next(0, ambientSounds.Count);

        ambientSounds[a].Play();
    }
}

