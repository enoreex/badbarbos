using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Badbarbos
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;

        [Header("UI Elements")]
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private string _volumeSliderObjectName = "volumeSlider";

        [Header("Scene Names")]
        [SerializeField] private string _noMusicSceneName = "NoMusicScene";
        [SerializeField] private string _settingsSceneName = "SettingsScene";

        [Header("PlayerPrefs Keys")]
        [SerializeField] private string _volumePrefKey = "MasterVolume";

        [Header("Settings")]
        [SerializeField, Range(0f, 1f)] private float _defaultVolume = 1f;

        private float _currentVolume;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _currentVolume = PlayerPrefs.GetFloat(_volumePrefKey, _defaultVolume);
            ApplyVolume(_currentVolume);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            bool playMusic = scene.name != _noMusicSceneName;

            SetMusicActive(playMusic);

            if (scene.name == _settingsSceneName)
            {
                var sliderObj = GameObject.Find(_volumeSliderObjectName);
                if (sliderObj != null)
                {
                    _volumeSlider = sliderObj.GetComponent<Slider>();
                    if (_volumeSlider != null)
                    {
                        _volumeSlider.SetValueWithoutNotify(_currentVolume);
                        _volumeSlider.onValueChanged.RemoveAllListeners();
                        _volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
                    }
                }
            }
            else
            {
                _volumeSlider = null;
            }
        }

        public void SetMusicActive(bool isActive)
        {
            if (_musicSource != null)
                _musicSource.gameObject.SetActive(isActive);
        }

        private void OnVolumeSliderChanged(float volume)
        {
            _currentVolume = volume;
            ApplyVolume(volume);
            PlayerPrefs.SetFloat(_volumePrefKey, volume);
            PlayerPrefs.Save();
        }

        private void ApplyVolume(float volume)
        {
            AudioListener.volume = volume;
            if (_musicSource != null)
                _musicSource.volume = volume;
        }
    }
}