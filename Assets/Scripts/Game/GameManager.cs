using System.Collections;
using System.Net.Mime;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class GameManager : Singleton<GameManager>
    {
        private ScoreHandler _scoreHandler;
        private OverlayController OverlayController => OverlayController.Instance;

        public long Score => _scoreHandler.Get();

        /// <summary>
        /// </summary>
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Game")
            {
                _scoreHandler = new ScoreHandler();
                CameraController.Instance.miniCamera.enabled = true;
                
                var quitButton = GameObject.Find("QuitButton")
                    .GetComponent<Button>();
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(() => Instantiate("Menu"));
                
                var tryAgainButton = GameObject.Find("TryAgainButton")
                    .GetComponent<Button>();
                tryAgainButton.onClick.RemoveAllListeners();
                tryAgainButton.onClick.AddListener(() => Instantiate("Game"));
                

            }
        }

        /// <summary>
        /// </summary>
        private void Update()
        {
            if (SceneManager.GetActiveScene().name != "Game") return;

            if (Input.GetKeyUp(KeyCode.Escape)) Instantiate("Menu");
        }

        /// <summary>
        /// </summary>
        /// <param name="scene"></param>
        public void Instantiate(string scene)
        {
            StartCoroutine(LoadScene(scene));
        }

        /// <summary>
        /// 
        /// </summary>
        public void HandleEndOfGame(bool isGameOver)
        {
            var gameOverPanel = GameObject.Find("GameOverPanel").GetComponent<Hideable>();
            var text = gameOverPanel.transform.Find("Head").GetComponent<TextMeshProUGUI>();
            var score = gameOverPanel.transform.Find("ScoreText/ScoreFinal")
                .GetComponent<Text>();

            text.text = isGameOver ? "Game Over" : "Congratulations";
            score.text = Score.ToString();
            CameraController.Instance.miniCamera.enabled = false;
            gameOverPanel.Show();
        }

        /// <summary>
        /// </summary>
        public void AddPoint()
        {
            _scoreHandler.Add();
            OverlayController.SetScore(_scoreHandler.Get());
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetScore()
        {
            _scoreHandler.Reset();
            OverlayController.SetScore(_scoreHandler.Get());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds"></param>
        public void SetTimeCurrent(float seconds)
        {
            OverlayController.SetTimeCurrent(seconds);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds"></param>
        public void SetTimeTotal(float seconds)
        {
            OverlayController.SetTimeTotal(seconds);
        }

        /// <summary>
        ///     Load the provided scene asynchronously.
        /// </summary>
        /// <param name="scene">The scene to load.</param>
        /// <returns>A coroutine.</returns>
        private IEnumerator LoadScene(string scene)
        {
            var operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            operation.allowSceneActivation = false;
            while (operation.progress < 0.9f)
                yield return null;

            if (operation.progress >= 0.9f) operation.allowSceneActivation = true;
        }
    }
}