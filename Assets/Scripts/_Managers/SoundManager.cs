using System;
using MelenitasDev.SoundsGood;
using UnityEngine;
 
public class SoundManager : MonoBehaviour
{
    private Music music;
    
    public void Init()
    {
        
    }

    private void Start()
    {
        var set = SROptions.Current.Master;
        var set2 = SROptions.Current.Sfx;
        var set3 = SROptions.Current.Music;
    }

    public void PlayMusic(Track track)
    {
        if(music!= null)
            music.Stop(1.5f);
        music = new Music(track);
        music
            .SetOutput(Output.Music)
            .SetLoop(true)
            .Play(1.5f);
    }

    public void StopMusic()
    {
        music?.Stop(1.5f);
    }
    
    public void PlaySfx(SFX sfx)
    {
        switch (sfx)
        {
            default:
                Sound sound = new Sound(sfx);
                sound
                    .SetOutput(Output.SFX)
                    .Play();
                break;
        }
    }
}