using UnityEngine;
 
public class SoundManager : MonoBehaviour
{
    private AudioListener _listener;

    private AudioSource _bgmAudioSource;
    private AudioSource _sfxAudioSource;
    private AudioSource _voiceAudioSource;
    
    private const string RootPath = "Sound/";
    private const string BGMPath = RootPath + "Bgm/";
    private const string SfxPath = RootPath + "Sfx/";
    private const string VoicePath = RootPath + "Voice/";

    [SerializeField] private float allVolume = 1;
    [SerializeField] private float bgmVolume = 1;
    [SerializeField] private float sfxVolume = 1;
    [SerializeField] private float voiceVolume = 1;
    
    public void Init()
    {
        SetListenerToThisObject();
        _bgmAudioSource = CreateAudioSource("bgmAudioSource", true, false);
        _sfxAudioSource = CreateAudioSource("sfxAudioSource", false, false);
        _voiceAudioSource = CreateAudioSource("voiceAudioSource", false, false);
    }
    
    public void PlaySfx(SfxEnum file, bool stopBeforeSound = true)
    {
        AudioClip clip = Resources.Load<AudioClip>(BGMPath + file.ToFileInfo(out var individualVolume));
        if (clip == null)
        {
            Debug.LogError($"[{file}]사운드를 찾을 수 없습니다.");
            return;
        }

        PlaySound(ref _sfxAudioSource, clip, stopBeforeSound, sfxVolume, individualVolume);
    }
    
    public void PlayVoice(VoiceEnum file, bool stopBeforeSound = true)
    {
        AudioClip clip = Resources.Load<AudioClip>( SfxPath + file.ToFileInfo(out var individualVolume));
        if (clip == null)
        {
            Debug.LogError($"[{file}]사운드를 찾을 수 없습니다.");
            return;
        }

        PlaySound(ref _voiceAudioSource, clip, stopBeforeSound, voiceVolume, individualVolume);
    }
    
    public void PlayBGM(BgmEnum file)
    {
        AudioClip clip = Resources.Load<AudioClip>(BGMPath + file.ToFileInfo(out var individualVolume));
        if (clip == null)
        {
            Debug.LogError($"[{file}]사운드를 찾을 수 없습니다.");
            return;
        }

        PlaySound(ref _bgmAudioSource, clip, true, bgmVolume, individualVolume);
    }

    public void StopBGM()
    {
        _bgmAudioSource.Stop();
    }

    private void PlaySound(ref AudioSource targetAudioSource, AudioClip clip, bool stopBeforeSound = true, float volume = 1f, float optionVolume = 1f, float pitch = 1f, bool isLoop = false)
    {
        if (_listener == null && !_listener.enabled && !_listener.gameObject.activeInHierarchy)
            return;
            
        
        if (targetAudioSource != null)
        {
            targetAudioSource.priority = 50;
            targetAudioSource.pitch = pitch;
            targetAudioSource.loop = isLoop;
            targetAudioSource.clip = clip;
            if (stopBeforeSound)
                targetAudioSource.Play();
            else
                targetAudioSource.PlayOneShot(clip, (volume * optionVolume * allVolume));
        }
    }
    
    
    /// <summary>
    /// audio listener를 사운드 매니저에 귀속
    /// </summary>
    private void SetListenerToThisObject()
    {
        if (_listener != null)
            return;
        
        Camera cam = Camera.main;
        if (cam == null)
            cam = FindObjectOfType(typeof(Camera)) as Camera;

        if (cam == null)
            return;
        
        if (cam.GetComponent<AudioListener>() != null)
            Destroy(cam.GetComponent<AudioListener>());
        _listener = gameObject.GetOrAddComponent<AudioListener>();
    }
    
    private AudioSource CreateAudioSource(string audioSourceName, bool loop, bool playOnAwake)
    {
        GameObject go = new GameObject(audioSourceName);
        go.transform.SetParent(transform);
        var audioSource = go.AddComponent<AudioSource>();
        audioSource.priority = 50;
        audioSource.pitch = 1f;
        audioSource.loop = loop;
        audioSource.playOnAwake = playOnAwake;
        return audioSource;
    }
}