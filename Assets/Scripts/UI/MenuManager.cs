using System.Linq;
using Audio;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// This class is used to handle all interactions within the Menu scene.
    /// </summary>
    public class MenuManager : Singleton<MenuManager>
    {
        private MenuType _current;
        [SerializeField] private Hideable mainMenu;
        [SerializeField] private Hideable playMenu;
        [SerializeField] private Hideable aboutMenu;

        private Button _ownButton => GameObject.Find("Own").GetComponent<Button>();
        private Button _playButton => GameObject.Find("PlayButton").GetComponent<Button>();

        private static FileHandler FileHandler => FileHandler.Instance;

        /// <summary>
        /// When script is loaded, show main menu and reset it to a default state.
        /// </summary>
        private void Start()
        {
            ShowMainMenu();
            ResetAudioState();
            UpdatePlayMenu();
        }

        /// <summary>
        /// Checks if PlayButton can be made interactable in every frame, which depends on if the user chose a song.
        /// Also checks if ESC button was hit.
        /// </summary>
        private void Update()
        {
            _playButton.interactable = playMenu.GetComponentsInChildren<Button>()
                .Any(button => EventSystem.current.currentSelectedGameObject == button.gameObject);

            if (Input.GetKeyUp(KeyCode.Escape)) ProcessEscape();
        }

        /// <summary>
        /// Hides main menu and play menu.
        /// </summary>
        private void HideAll()
        {
            mainMenu.Hide();
            playMenu.Hide();
            aboutMenu.Hide();
        }

        /// <summary>
        /// Shows main menu to the user.
        /// </summary>
        public void ShowMainMenu()
        {
            HideAll();
            mainMenu.Show();
            _current = MenuType.MainMenu;
        }

        /// <summary>
        /// Shows play menu to the user.
        /// </summary>
        public void ShowPlayMenu()
        {
            HideAll();
            playMenu.Show();
            _current = MenuType.PlayMenu;
        }

        /// <summary>
        /// Shows about menu to the user.
        /// </summary>
        public void ShowAboutMenu()
        {
            HideAll();
            aboutMenu.Show();
            _current = MenuType.AboutMenu;
        }

        /// <summary>
        /// Selects the default song to be used within the game.
        /// </summary>
        public void SelectDefault()
        {
            FileHandler.clip = Resources.Load<AudioClip>("Audio/default");
            _playButton.onClick.RemoveAllListeners();
            _playButton.onClick.AddListener(delegate { GameManager.Instance.Instantiate("Game"); });
        }

        /// <summary>
        /// Updates the play menu depending on the chosen song.
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
        /// Resets the 'Choose Song' button when play menu is reopened.
        /// </summary>
        private void ResetAudioState()
        {
            FileHandler.clip = null;
            _ownButton.GetComponentInChildren<Text>().text = "C H O O S E   S O N G";
            _ownButton.onClick.RemoveAllListeners();
            _ownButton.onClick.AddListener(FileHandler.Select);
        }

        /// <summary>
        /// Processes the hit of the ESC button depending on the current state.
        /// On Android, it is called when the Back button is hit.
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
                case MenuType.AboutMenu:
                    ShowMainMenu();
                    break;
            }
        }
    }

    /// <summary>
    /// Simple enumerator to keep track of the current menu the user is in.
    /// </summary>
    internal enum MenuType
    {
        MainMenu,
        PlayMenu,
        AboutMenu
    }
}