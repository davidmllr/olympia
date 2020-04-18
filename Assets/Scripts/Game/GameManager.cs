using System.Collections;
using System.Net.Mime;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// This class is used as a game manager.
    /// It handles points and things in the overlay.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        private ScoreHandler _scoreHandler;
        private OverlayController OverlayController => OverlayController.Instance;

        public long Score => _scoreHandler.Get();

        /// <summary>
        /// When class in enabled, add a listener that listens for scene changes.
        /// </summary>
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// <summary>
        /// Gets called whenever a new scene is loaded.
        /// If scene is the game scene, the game over screen is instantiated silently in the background.
        /// Also, the mini camera is enabled and our ScoreHandler is instantiated.
        /// </summary>
        /// <param name="scene">Name of the scene that was loaded</param>
        /// <param name="mode">Mode in which the scene was loaded</param>
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
        /// Handles the use of the ESC button by the user when game scene is active.
        /// </summary>
        private void Update()
        {
            if (SceneManager.GetActiveScene().name != "Game") return;

            if (Input.GetKeyUp(KeyCode.Escape)) Instantiate("Menu");
        }

        /// <summary>
        /// Starts a coroutine that loads a new scene.
        /// </summary>
        /// <param name="scene">Name of the scene that shall be loaded</param>
        public void Instantiate(string scene)
        {
            StartCoroutine(LoadScene(scene));
        }

        /// <summary>
        /// Gets called when the game ends.
        /// Handles the layout of the GameOverPanel and shows it to the user afterwards.
        /// </summary>
        /// <param name="isGameOver">If game was ended early</param>
        public void HandleEndOfGame(bool isGameOver)
        {
            var gameOverPanel = GameObject.Find("GameOverPanel").GetComponent<Hideable>();
            var text = gameOverPanel.transform.Find("Head").GetComponent<Text>();
            var score = gameOverPanel.transform.Find("ScoreText/ScoreFinal")
                .GetComponent<Text>();

            text.text = isGameOver ? "Game Over" : "Congratulations";
            score.text = Score.ToString();
            CameraController.Instance.miniCamera.enabled = false;
            gameOverPanel.Show();
        }

        /// <summary>
        /// Adds a point to the ScoreHandler and shows it to the user.
        /// </summary>
        public void AddPoint()
        {
            _scoreHandler.Add();
            OverlayController.SetScore(_scoreHandler.Get());
        } 
        
        /// <summary>
        /// Remove a point from the ScoreHandler and shows it to the user.
        /// </summary>
        public void RemovePoint()
        {
            _scoreHandler.Remove();
            OverlayController.SetScore(_scoreHandler.Get());
        }
        /// <summary>
        /// Resets the score in the ScoreHandler and shows it to the user.
        /// </summary>
        public void ResetScore()
        {
            _scoreHandler.Reset();
            OverlayController.SetScore(_scoreHandler.Get());
        }

        /// <summary>
        /// Sets the current time in the overlay.
        /// </summary>
        /// <param name="seconds">The current time in seconds</param>
        public void SetTimeCurrent(float seconds)
        {
            OverlayController.SetTimeCurrent(seconds);
        }
        
        /// <summary>
        /// Sets the total time in the overlay.
        /// </summary>
        /// <param name="seconds">The total time in seconds</param>
        public void SetTimeTotal(float seconds)
        {
            OverlayController.SetTimeTotal(seconds);
        }

        /// <summary>
        /// Loads the provided scene asynchronously.
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