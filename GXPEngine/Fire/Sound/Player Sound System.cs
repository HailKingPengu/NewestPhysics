using GXPEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Player_Sound_System : GameObject
{
    Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();

    public Player_Sound_System()
    {
        foreach(string fileName in Directory.GetFiles("../../Assets/PlayerSounds"))
        {
            if(File.Exists(fileName))
            {
                sounds.Add(Path.GetFileNameWithoutExtension("../../Assets/PlayerSounds/" + fileName), new Sound(fileName));
            }
        }
    }

    public void Play(string key)
    {
        sounds.TryGetValue(key, out var sound);
        sound?.Play();
    }
}

