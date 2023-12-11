using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MelenitasDev.SoundsGood;
using UnityEngine;

public partial class SROptions {
    // Default Value for property
    private float _master = 0.5f;
    [Category("Audio")] 
    [NumberRange(0f, 1f)]
    [Increment(0.05f)]
    public float Master {
        get
        {
            _master = Managers.Save.LocalLoad<float>("Master", 1);
            AudioManager.ChangeOutputVolume(Output.Master, _master);
            return _master;
        }
        set
        {
            Managers.Save.LocalSave("Master", value);
            AudioManager.ChangeOutputVolume(Output.Master, _master);
            _master = value;
        }
    }
	
    // Default Value for property
    private float _sfx = 0.5f;
    [Category("Audio")] 
    [NumberRange(0f, 1f)]
    [Increment(0.05f)]
    public float Sfx {
        get
        {
            _sfx = Managers.Save.LocalLoad<float>("Sfx", 1f);
            AudioManager.ChangeOutputVolume(Output.SFX, _sfx);
            return _sfx;
        }
        set
        {
            Managers.Save.LocalSave("Sfx", value);
            AudioManager.ChangeOutputVolume(Output.SFX, _sfx);
            _sfx = value;
        }
    }
    
    // Default Value for property
    private float _music = 0;
    [Category("Audio")] 
    [NumberRange(0f, 1f)]
    [Increment(0.05f)]
    public float Music {
        get
        {
            _music = Managers.Save.LocalLoad<float>("Music", 1);
            AudioManager.ChangeOutputVolume(Output.Music, _music);
            return _music;
        }
        set
        {
            Managers.Save.LocalSave("Music", value);
            AudioManager.ChangeOutputVolume(Output.Music, _music);
            _music = value;
        }
    }
    
    [Category("Playing")]
    [NumberRange(10,5000)]
    [Increment(1f)]
    public float WaitTIme
    {
        get => Managers.Save.LocalLoad<float>("WaitTIme", 15f);
        set => Managers.Save.LocalSave("WaitTIme", value);
    }
}

