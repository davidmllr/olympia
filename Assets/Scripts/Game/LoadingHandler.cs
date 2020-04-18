using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>
    /// This class is called in the Preload scene to load the Menu scene.
    /// A preview scene is needed to preload some of the singletons used in this game.
    /// </summary>
    public class LoadingHandler : Singleton<LoadingHandler>
    {
        /// <summary>
        /// Load the menu scene.
        /// </summary>
        private void Start()
        {
            SceneManager.LoadScene("Menu");
        }
    }
}