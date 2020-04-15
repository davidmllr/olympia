using System.Linq;
using Audio;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class MenuManager : Singleton<MenuManager>
    {
        private MenuType _current;
        [SerializeField] private Hideable mainMenu;
        [SerializeField] private Hideable playMenu;

        private Button _ownButton => GameObject.Find("Own").GetComponent<Button>();
        private Button _playButton => GameObject.Find("PlayButton").GetComponent<Button>();

        private static FileHandler FileHandler => FileHandler.Instance;

        /// <summary>
        /// </summary>
        private void Start()
        {
            ShowMainMenu();
            ResetAudioState();
            UpdatePlayMenu();
        }

        /// <summary>
        /// </summary>
        private void Update()
        {
            _playButton.interactable = playMenu.GetComponentsInChildren<Button>()
                .Any(button => EventSystem.current.currentSelectedGameObject == button.gameObject);

            if (Input.GetKeyUp(KeyCode.Escape)) ProcessEscape();
        }

        /// <summary>
        /// </summary>
        private void HideAll()
        {
            mainMenu.Hide();
            playMenu.Hide();
        }

        /// <summary>
        /// </summary>
        public void ShowMainMenu()
        {
            HideAll();
            mainMenu.Show();
            _current = MenuType.MainMenu;
        }

        /// <summary>
        /// </summary>
        public void ShowPlayMenu()
        {
            HideAll();
            playMenu.Show();
            _current = MenuType.PlayMenu;
        }

        /// <summary>
        /// </summary>
        public void SelectDefault()
        {
            FileHandler.clip = Resources.Load<AudioClip>("Audio/default");
            _playButton.onClick.RemoveAllListeners();
            _playButton.onClick.AddListener(delegate { GameManager.Instance.Instantiate("Game"); });
        }

        /// <summary>
        /// </summary>
        public void UpdatePlayMenu()
        {
            if (FileHandler.clip != null)
            {
                _ownButton.GetComponentInChildren<Text>().text = FileHandler.clip.name;
                _ownButton.Select();
                _playButton.onClick.RemoveAllListeners();
                _playButton.onClick.AddListener(delegate { GameManager.Instance.Instantiate("Game"); });
            }
        }

        /// <summary>
        /// </summary>
        private void ResetAudioState()
        {
            FileHandler.clip = null;
            _ownButton.GetComponentInChildren<Text>().text = "C H O O S E   S O N G";
            _ownButton.onClick.RemoveAllListeners();
            _ownButton.onClick.AddListener(FileHandler.Select);
        }

        /// <summary>
        /// </summary>
        private void ProcessEscape()
        {
            switch (_current)
            {
                case MenuType.MainMenu:
                    Application.Quit();
                    break;
                case MenuType.PlayMenu:
                    ResetAudioState();
                    ShowMainMenu();
                    UpdatePlayMenu();
                    break;
            }
        }
    }

    /// <summary>
    /// </summary>
    internal enum MenuType
    {
        MainMenu,
        PlayMenu
    }
}