using System;
using System.Collections;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GMTK.MainMenu
{
    /// <summary>
    /// Controls the audio options UI (BGM & SFX sliders) by driving the MMSoundManagerSettingsSO,
    /// which writes the exposed AudioMixer parameters (MusicVolume / SfxVolume) and persists them.
    /// Works with or without an MMSoundManager in the scene, since the settings asset owns the mixer.
    /// Slider values are in the 0..1 range and map directly to the normalized track volume (0 = silent).
    /// </summary>
    public class OptionsUIController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Sound Settings")]
        [Tooltip("The MMSoundManagerSettings asset holding the AudioMixer and its exposed parameters. " +
                 "If left empty, the settings of the MMSoundManager instance are used instead.")]
        [SerializeField] private MMSoundManagerSettingsSO _soundSettings;

        [Header("Sliders")]
        [Tooltip("Slider that controls the BGM (Music track) volume.")]
        [SerializeField] private Slider _bgmSlider;

        [Tooltip("Slider that controls the SFX track volume.")]
        [SerializeField] private Slider _sfxSlider;

        [Header("Value Labels (Optional)")]
        [Tooltip("Optional label showing the current BGM volume.")]
        [SerializeField] private TMP_Text _bgmValueLabel;

        [Tooltip("Optional label showing the current SFX volume.")]
        [SerializeField] private TMP_Text _sfxValueLabel;

        [Tooltip("Format used by the value labels. {0} is the volume expressed as 0..100.")]
        [SerializeField] private string _valueFormat = "{0:0}%";

        [Header("Defaults")]
        [Tooltip("BGM volume applied by ResetToDefaults().")]
        [Range(0f, 1f)]
        [SerializeField] private float _defaultBgmVolume = 0.2f;

        [Tooltip("SFX volume applied by ResetToDefaults().")]
        [Range(0f, 1f)]
        [SerializeField] private float _defaultSfxVolume = 1f;

        [Header("Saving")]
        [Tooltip("If true, loads the saved settings and applies them to the mixer when this UI first starts.")]
        [SerializeField] private bool _loadOnStart = true;

        [Tooltip("Seconds to wait after the last change before writing the save file. Avoids saving every frame while dragging a slider.")]
        [Range(0f, 2f)]
        [SerializeField] private float _saveDelay = 0.35f;

        [Header("Debug")]
        [Tooltip("Prints diagnostic logs to the console when true.")]
        [SerializeField] private bool _enableLogging = false;

        #endregion

        #region Events

        /// <summary>Raised when the BGM volume changes (0..1).</summary>
        public event Action<float> OnBgmVolumeChanged;

        /// <summary>Raised when the SFX volume changes (0..1).</summary>
        public event Action<float> OnSfxVolumeChanged;

        #endregion

        #region Fields

        private MMSoundManagerSettingsSO _resolvedSettings;
        private Coroutine _saveCoroutine;
        private bool _savePending;
        private bool _initialized;
        private bool _initializing;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            ConfigureSlider(_bgmSlider);
            ConfigureSlider(_sfxSlider);
        }

        // Applied on Start rather than Awake: AudioMixer.SetFloat is unreliable during Awake.
        private void Start()
        {
            if (!ResolveSettings()) return;

            if (_loadOnStart)
            {
                _resolvedSettings.LoadSoundSettings();
            }

            // Pushes the current settings values into the mixer, so the mixer, the sliders
            // and the settings asset all agree even when no save file exists yet.
            _initializing = true;
            ApplyVolume(MMSoundManager.MMSoundManagerTracks.Music, _resolvedSettings.Settings.MusicVolume, false);
            ApplyVolume(MMSoundManager.MMSoundManagerTracks.Sfx, _resolvedSettings.Settings.SfxVolume, false);
            _initializing = false;

            _initialized = true;
            RefreshFromSettings();
        }

        private void OnEnable()
        {
            if (_bgmSlider != null) _bgmSlider.onValueChanged.AddListener(HandleBgmSliderChanged);
            if (_sfxSlider != null) _sfxSlider.onValueChanged.AddListener(HandleSfxSliderChanged);

            if (_initialized)
            {
                RefreshFromSettings();
            }
        }

        private void OnDisable()
        {
            if (_bgmSlider != null) _bgmSlider.onValueChanged.RemoveListener(HandleBgmSliderChanged);
            if (_sfxSlider != null) _sfxSlider.onValueChanged.RemoveListener(HandleSfxSliderChanged);

            FlushPendingSave();
        }

        private void OnApplicationQuit()
        {
            FlushPendingSave();
        }

        #endregion

        #region Public API

        /// <summary>Sets the BGM (Music track) volume in the 0..1 range and syncs the slider.</summary>
        public void SetBgmVolume(float normalizedVolume)
        {
            ApplyVolume(MMSoundManager.MMSoundManagerTracks.Music, normalizedVolume, true);
        }

        /// <summary>Sets the SFX track volume in the 0..1 range and syncs the slider.</summary>
        public void SetSfxVolume(float normalizedVolume)
        {
            ApplyVolume(MMSoundManager.MMSoundManagerTracks.Sfx, normalizedVolume, true);
        }

        /// <summary>Restores both tracks to their configured default volumes.</summary>
        public void ResetToDefaults()
        {
            SetBgmVolume(_defaultBgmVolume);
            SetSfxVolume(_defaultSfxVolume);
        }

        /// <summary>Writes the current settings to disk immediately.</summary>
        public void Save()
        {
            FlushPendingSave();
        }

        /// <summary>Pulls the current track volumes from the settings asset and updates the sliders and labels.</summary>
        public void RefreshFromSettings()
        {
            if (!ResolveSettings()) return;

            SyncUI(_bgmSlider, _bgmValueLabel, _resolvedSettings.Settings.MusicVolume);
            SyncUI(_sfxSlider, _sfxValueLabel, _resolvedSettings.Settings.SfxVolume);
        }

        #endregion

        #region Main Logic

        private void HandleBgmSliderChanged(float value)
        {
            ApplyVolume(MMSoundManager.MMSoundManagerTracks.Music, value, false);
        }

        private void HandleSfxSliderChanged(float value)
        {
            ApplyVolume(MMSoundManager.MMSoundManagerTracks.Sfx, value, false);
        }

        /// <summary>
        /// Applies a normalized volume to a track, updates the UI and schedules a save.
        /// </summary>
        /// <param name="syncSlider">False when the change originated from the slider itself, to avoid feedback loops.</param>
        private void ApplyVolume(MMSoundManager.MMSoundManagerTracks track, float normalizedVolume, bool syncSlider)
        {
            if (!ResolveSettings()) return;

            float clamped = Mathf.Clamp01(normalizedVolume);

            // A volume of 0 is pushed to the minimal volume by the settings asset, which maps to -80dB (silence).
            _resolvedSettings.SetTrackVolume(track, clamped);

            Slider slider = TrackSlider(track);
            SyncUI(syncSlider ? slider : null, TrackLabel(track), clamped);

            if (track == MMSoundManager.MMSoundManagerTracks.Music)
            {
                OnBgmVolumeChanged?.Invoke(clamped);
            }
            else
            {
                OnSfxVolumeChanged?.Invoke(clamped);
            }

            RequestSave();
            Log($"{track} volume set to {clamped:0.00}.");
        }

        #endregion

        #region Saving

        private void RequestSave()
        {
            // Nothing new to persist while pushing the already saved values back into the mixer.
            if (_initializing) return;

            if (!isActiveAndEnabled)
            {
                WriteSave();
                return;
            }

            _savePending = true;

            if (_saveCoroutine != null)
            {
                StopCoroutine(_saveCoroutine);
            }
            _saveCoroutine = StartCoroutine(SaveAfterDelay());
        }

        private IEnumerator SaveAfterDelay()
        {
            yield return new WaitForSecondsRealtime(_saveDelay);
            _saveCoroutine = null;
            WriteSave();
        }

        private void FlushPendingSave()
        {
            if (_saveCoroutine != null)
            {
                StopCoroutine(_saveCoroutine);
                _saveCoroutine = null;
            }

            if (_savePending)
            {
                WriteSave();
            }
        }

        private void WriteSave()
        {
            if (_resolvedSettings == null) return;

            _savePending = false;
            _resolvedSettings.SaveSoundSettings();
            Log("Sound settings saved.");
        }

        #endregion

        #region Internal Helpers

        /// <summary>Resolves the settings asset, falling back to the MMSoundManager instance when none is assigned.</summary>
        private bool ResolveSettings()
        {
            if (_resolvedSettings != null) return true;

            if (_soundSettings != null)
            {
                _resolvedSettings = _soundSettings;
            }
            else if (MMSoundManager.HasInstance && MMSoundManager.Instance.settingsSo != null)
            {
                _resolvedSettings = MMSoundManager.Instance.settingsSo;
            }

            if (_resolvedSettings == null)
            {
                Debug.LogWarning("[OptionsUIController] No MMSoundManagerSettings asset assigned and no MMSoundManager in the scene; audio options are inactive.", this);
                return false;
            }

            if (_resolvedSettings.TargetAudioMixer == null)
            {
                Debug.LogWarning($"[OptionsUIController] '{_resolvedSettings.name}' has no TargetAudioMixer assigned; audio options are inactive.", this);
                _resolvedSettings = null;
                return false;
            }

            return true;
        }

        private Slider TrackSlider(MMSoundManager.MMSoundManagerTracks track)
        {
            return track == MMSoundManager.MMSoundManagerTracks.Music ? _bgmSlider : _sfxSlider;
        }

        private TMP_Text TrackLabel(MMSoundManager.MMSoundManagerTracks track)
        {
            return track == MMSoundManager.MMSoundManagerTracks.Music ? _bgmValueLabel : _sfxValueLabel;
        }

        private void ConfigureSlider(Slider slider)
        {
            if (slider == null) return;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
        }

        private void SyncUI(Slider slider, TMP_Text label, float value)
        {
            float clamped = Mathf.Clamp01(value);

            if (slider != null)
            {
                slider.SetValueWithoutNotify(clamped);
            }

            if (label != null)
            {
                label.text = string.Format(_valueFormat, clamped * 100f);
            }
        }

        private void Log(string message)
        {
            if (!_enableLogging) return;
            Debug.Log($"[OptionsUIController] {message}", this);
        }

        #endregion
    }
}
