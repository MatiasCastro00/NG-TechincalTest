using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundManager : NullableSingleton<SoundManager>
{
    [SerializeField] private AudioSource m_music;
    [SerializeField] private SoundList m_soundList;
    [SerializeField, Range(0f, 1f)] private float m_musicVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float m_fbxVolume = 1f;

    private List<AudioSource> m_fbx = new List<AudioSource>();
    private void Start()
    {
        m_music.volume = m_musicVolume;
        m_soundList.MakeFBXDictionary();
        EventManager.Instance.PlayerStep1 += () => PlaySound(SoundsKeys.VFX_STEP_1_KEY);
        EventManager.Instance.PlayerStep2 += () => PlaySound(SoundsKeys.VFX_STEP_2_KEY);
        EventManager.Instance.PlayerStep3 += () => PlaySound(SoundsKeys.VFX_STEP_3_KEY);
        EventManager.Instance.OpenGate += () => PlaySound(SoundsKeys.OPEN_GATE_VFX_KEY);
        EventManager.Instance.Talk += () => PlaySound(SoundsKeys.VOICE_KEY);
    }

    public void PlaySound(string soundName)
    {
        AudioClip clip = m_soundList.fbxDictionary[soundName];
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: the clip is null.");
            return;
        }

        AudioSource availableSource = null;

        foreach (var source in m_fbx)
        {
            if (!source.isPlaying)
            {
                availableSource = source;
                break;
            }
        }
        if (availableSource == null)
        {
            GameObject newSourceGO = new GameObject("SFX_AudioSource");
            newSourceGO.transform.SetParent(transform);
            availableSource = newSourceGO.AddComponent<AudioSource>();
            availableSource.playOnAwake = false;
            availableSource.volume = m_fbxVolume;
            m_fbx.Add(availableSource);
        }

        availableSource.clip = clip;
        availableSource.volume = m_fbxVolume;
        availableSource.Play();
    }

    public void PlayMusic(AudioClip musicClip, bool loop = true)
    {
        if (musicClip == null)
        {
            Debug.LogWarning("SoundManager: the clip music is null.");
            return;
        }

        m_music.clip = musicClip;
        m_music.loop = loop;
        m_music.volume = m_musicVolume;
        m_music.Play();
    }

    public void StopMusic()
    {
        m_music.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        m_musicVolume = Mathf.Clamp01(volume);
        m_music.volume = m_musicVolume;
    }

    public void SetSfxVolume(float volume)
    {
        m_fbxVolume = Mathf.Clamp01(volume);
        foreach (var source in m_fbx)
        {
            if (!source.isPlaying) source.volume = m_fbxVolume;
        }
    }
}
