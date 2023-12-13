using System;
using MelenitasDev.SoundsGood;
using UnityEngine;
 
public class SoundManager : MonoBehaviour
{
    private Music music;
    private float sandStepTimer;
    private float seatStepTimer;
    private float stepSoundInterval = 0.6f;
    
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
            case SFX.SeatStep:
                if (Mathf.Abs(seatStepTimer - Time.realtimeSinceStartup) < SROptions.Current.StepSoundMinInterval)
                    return;
                seatStepTimer = Time.realtimeSinceStartup;
                Sound seatStepSound = new Sound(sfx);
                seatStepSound
                    .SetOutput(Output.SFX)
                    .SetVolume(AudioManager.GetLastSavedOutputVolume(Output.SFX) * SROptions.Current.StepSound)
                    .SetRandomClip(true)
                    .SetRandomPitch()
                    .Play();
                break;
            case SFX.SandStep:
                if (Mathf.Abs(sandStepTimer - Time.realtimeSinceStartup) < SROptions.Current.StepSoundMinInterval)
                    return;
                sandStepTimer = Time.realtimeSinceStartup;
                Sound sandStepSound = new Sound(sfx);
                sandStepSound
                    .SetVolume(AudioManager.GetLastSavedOutputVolume(Output.SFX) * SROptions.Current.StepSound)
                    .SetOutput(Output.SFX)
                    .SetRandomPitch()
                    .Play();
                break;
            default:
                Sound sound = new Sound(sfx);
                sound
                    .SetOutput(Output.SFX)
                    .Play();
                break;
        }
    }
}