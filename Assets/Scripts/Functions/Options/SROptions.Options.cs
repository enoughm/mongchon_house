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
            _sfx = Managers.Save.LocalLoad<float>("Sfx", 0.5f);
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
            _music = Managers.Save.LocalLoad<float>("Music", 0);
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

    [Category("Seats")]
    public bool ShowArea { get; set; } = false;
    
    [Category("Seats")]
    [Increment(0.2f)]
    public float BowlParticleYOffset
    {
        get => Managers.Save.LocalLoad<float>("BowlParticleYOffset", 2.1f);
        set => Managers.Save.LocalSave("BowlParticleYOffset", value);
    }
    
    [Category("Seats")]
    [Increment(0.2f)]
    public float BowlParticleXOffset
    {
        get => Managers.Save.LocalLoad<float>("BowlParticleXOffset", -0.76f);
        set => Managers.Save.LocalSave("BowlParticleXOffset", value);
    }
    
  
    [Category("Seats")]
    [Increment(1)]
    public float BowlAreaHeight
    {
        get => Managers.Save.LocalLoad<float>("BowlAreaHeight", 222);
        set => Managers.Save.LocalSave("BowlAreaHeight", value);
    }
    
    [Category("Seats")]
    [Increment(1)]
    public float BowlAreaWidth
    {
        get => Managers.Save.LocalLoad<float>("BowlAreaWidth", 210);
        set => Managers.Save.LocalSave("BowlAreaWidth", value);
    }
    
    [Category("Seats")]
    [Increment(1)]
    public float BowlAreaYOffset
    {
        get => Managers.Save.LocalLoad<float>("BowlAreaYOffset", 311);
        set => Managers.Save.LocalSave("BowlAreaYOffset", value);
    }
    
    [Category("Seats")]
    [Increment(1)]
    public float BowlAreaXOffset
    {
        get => Managers.Save.LocalLoad<float>("BowlAreaXOffset", 3067);
        set => Managers.Save.LocalSave("BowlAreaXOffset", value);
    }
    
    
    [Category("Seats")]
    [Increment(1)]
    public float PlateAreaHeight
    {
        get => Managers.Save.LocalLoad<float>("PlateAreaHeight", 95);
        set => Managers.Save.LocalSave("PlateAreaHeight", value);
    }
    
    [Category("Seats")]
    [Increment(1)]
    public float PlateAreaWidth
    {
        get => Managers.Save.LocalLoad<float>("PlateAreaWidth", 210);
        set => Managers.Save.LocalSave("PlateAreaWidth", value);
    }
    
    [Category("Seats")]
    [Increment(1)]
    public float PlateAreaYOffset
    {
        get => Managers.Save.LocalLoad<float>("PlateAreaYOffset", 293);
        set => Managers.Save.LocalSave("PlateAreaYOffset", value);
    }
    
    [Category("Seats")]
    [Increment(1)]
    public float PlateAreaXOffset
    {
        get => Managers.Save.LocalLoad<float>("PlateAreaXOffset", 3525);
        set => Managers.Save.LocalSave("PlateAreaXOffset", value);
    }
}

