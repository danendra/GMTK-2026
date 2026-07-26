using MoreMountains.Tools;
using UnityEngine;

namespace AudioSystem
{
    /// <summary>
    /// Loads the saved BGM/SFX volumes and pushes them into the AudioMixer when the game starts.
    /// Needed because no MMSoundManager lives in the game scenes: without this, the mixer would keep
    /// the values baked in the asset until the player opens the options menu.
    /// Put this on the ====SOUND==== prefab so every scene applies the player's saved volumes.
    /// </summary>
    public class AudioSettingsBootstrap : MonoBehaviour
    {
        [Tooltip("The MMSoundManagerSettings asset holding the AudioMixer and its exposed parameters.")]
        [SerializeField] private MMSoundManagerSettingsSO _soundSettings;

        [Tooltip("Prints diagnostic logs to the console when true.")]
        [SerializeField] private bool _enableLogging = false;

        // Applied on Start rather than Awake: AudioMixer.SetFloat is unreliable during Awake.
        private void Start()
        {
            if (_soundSettings == null)
            {
                Debug.LogWarning("[AudioSettingsBootstrap] No MMSoundManagerSettings asset assigned; saved volumes will not be applied.", this);
                return;
            }

            if (_soundSettings.TargetAudioMixer == null)
            {
                Debug.LogWarning($"[AudioSettingsBootstrap] '{_soundSettings.name}' has no TargetAudioMixer assigned; saved volumes will not be applied.", this);
                return;
            }

            _soundSettings.LoadSoundSettings();

            // Re-applies the values to the mixer, which also covers the first launch (no save file yet).
            _soundSettings.SetTrackVolume(MMSoundManager.MMSoundManagerTracks.Music, _soundSettings.Settings.MusicVolume);
            _soundSettings.SetTrackVolume(MMSoundManager.MMSoundManagerTracks.Sfx, _soundSettings.Settings.SfxVolume);

            if (_enableLogging)
            {
                Debug.Log($"[AudioSettingsBootstrap] Applied BGM {_soundSettings.Settings.MusicVolume:0.00} / SFX {_soundSettings.Settings.SfxVolume:0.00}.", this);
            }
        }
    }
}
