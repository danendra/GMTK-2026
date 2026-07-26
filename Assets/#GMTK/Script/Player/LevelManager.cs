using UnityEngine;
using AudioSystem;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [SerializeField] protected SoundData minionSoundData;
    [SerializeField] protected SoundData bossSoundData;
    [SerializeField] protected float fadeDuration = 1f;

    AudioSource minionSource;
    AudioSource bossSource;
    Coroutine transitionCoroutine;

    void Awake()
    {
        EnsureAudioSources();
    }

    public void PlayMinionBgm()
    {
        PlayBgm(false);
    }

    public void PlayBossBgm()
    {
        PlayBgm(true);
    }

    public void PlayBgm(bool bossReached)
    {
        EnsureAudioSources();

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(IEPlayBgm(bossReached));
    }

    IEnumerator IEPlayBgm(bool bossReached)
    {
        AudioSource toPlay = bossReached ? bossSource : minionSource;
        AudioSource toFadeOut = bossReached ? minionSource : bossSource;
        SoundData targetData = bossReached ? bossSoundData : minionSoundData;

        if (targetData == null)
        {
            transitionCoroutine = null;
            yield break;
        }

        bool sameClipAlreadyPlaying = toPlay.isPlaying && toPlay.clip == targetData.Clip;
        if (!sameClipAlreadyPlaying)
        {
            ConfigureSource(toPlay, targetData);
            toPlay.volume = 0f;
            toPlay.Play();
        }

        float duration = Mathf.Max(0.01f, fadeDuration);
        float elapsed = 0f;
        float fromStart = toFadeOut.volume;
        float toStart = toPlay.volume;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            toFadeOut.volume = Mathf.Lerp(fromStart, 0f, t);
            toPlay.volume = Mathf.Lerp(toStart, targetData.volume, t);
            yield return null;
        }

        toFadeOut.volume = 0f;
        toFadeOut.Stop();
        toPlay.volume = targetData.volume;
        transitionCoroutine = null;
    }

    void EnsureAudioSources()
    {
        if (minionSource == null)
        {
            minionSource = gameObject.AddComponent<AudioSource>();
        }

        if (bossSource == null)
        {
            bossSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void ConfigureSource(AudioSource source, SoundData data)
    {
        source.clip = data.Clip;
        source.outputAudioMixerGroup = data.MixerGroup;
        source.loop = true;
        source.playOnAwake = false;

        source.mute = data.mute;
        source.bypassEffects = data.bypassEffects;
        source.bypassListenerEffects = data.bypassListenerEffects;
        source.bypassReverbZones = data.bypassReverbZones;

        source.priority = data.priority;
        source.volume = data.volume;
        source.pitch = data.pitch;
        source.panStereo = data.panStereo;
        source.spatialBlend = data.spatialBlend;
        source.reverbZoneMix = data.reverbZoneMix;
        source.dopplerLevel = data.dopplerLevel;
        source.spread = data.spread;

        source.minDistance = data.minDistance;
        source.maxDistance = data.maxDistance;

        source.ignoreListenerVolume = data.ignoreListenerVolume;
        source.ignoreListenerPause = data.ignoreListenerPause;
        source.rolloffMode = data.rolloffMode;
    }
}
