using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Badbarbos.Menu
{
    public class MenuController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup _fadeGroup;

        [Header("Scene Names")]
        [SerializeField] private string _gameSceneName = "MenuGame";
        [SerializeField] private string _settingsSceneName = "MenuSettings";
        [SerializeField] private string _createLobbySceneName = "MenuCreateLobby";
        [SerializeField] private string _joinLobbySceneName = "MenuJoinLobby";
        [SerializeField] private string _mainMenuSceneName = "MenuMain";

        [Header("Fade Settings")]
        [SerializeField] private float _fadeDuration = 1f;

        private void Start()
        {
            StartCoroutine(FadeIn());
        }

        private void FadeToScene(string sceneName)
        {
            StartCoroutine(FadeOut(sceneName));
        }

        private IEnumerator FadeIn()
        {
            float timer = _fadeDuration;
            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                _fadeGroup.alpha = timer / _fadeDuration;
                yield return null;
            }
            _fadeGroup.alpha = 0f;
        }

        private IEnumerator FadeOut(string sceneName)
        {
            float timer = 0f;
            while (timer < _fadeDuration)
            {
                timer += Time.deltaTime;
                _fadeGroup.alpha = timer / _fadeDuration;
                yield return null;
            }

            SceneManager.LoadScene(sceneName);
        }

        public void StartGame()
        {
            FadeToScene(_gameSceneName);
        }

        public void OpenSettings()
        {
            FadeToScene(_settingsSceneName);
        }

        public void CreateLobby()
        {
            FadeToScene(_createLobbySceneName);
        }

        public void JoinLobby()
        {
            FadeToScene(_joinLobbySceneName);
        }

        public void ReturnToMainMenu()
        {
            FadeToScene(_mainMenuSceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}