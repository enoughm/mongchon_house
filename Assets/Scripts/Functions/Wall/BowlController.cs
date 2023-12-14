using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MelenitasDev.SoundsGood;
using UniRx;
using UnityEngine;

public class BowlController : MonoBehaviour
{
    [SerializeField] private ParticleSystem _smokeParticle;
    
    private void Start()
    {
        
        this.ObserveEveryValueChanged(_=>Managers.Game.BowlDetector.IsSomething).Subscribe(OnIsSomethingChanged);
        this.ObserveEveryValueChanged(_=>SROptions.Current.BowlParticleXOffset).Subscribe(_=>OnChangeXOffset(_));
        this.ObserveEveryValueChanged(_=>SROptions.Current.BowlParticleYOffset).Subscribe(_=>OnChangeYOffset(_));
    }

    private void OnChangeYOffset(float f)
    {
        _smokeParticle.transform.localPosition = new Vector3(_smokeParticle.transform.localPosition.x, f, _smokeParticle.transform.localPosition.z);
    }

    private void OnChangeXOffset(float f)
    {
        _smokeParticle.transform.localPosition = new Vector3(f, _smokeParticle.transform.localPosition.y, _smokeParticle.transform.localPosition.z);
    }

    private void OnIsSomethingChanged(bool b)
    {
        if (b)
        {
            var mainModule = _smokeParticle.main;
            mainModule.loop = true;
            _smokeParticle.Play();
            Managers.Sound.PlaySfx(SFX.Fire);
        }
        else
        {
            var mainModule = _smokeParticle.main;
            mainModule.loop = false;
            Managers.Sound.StopFire();
        }
    }
}
