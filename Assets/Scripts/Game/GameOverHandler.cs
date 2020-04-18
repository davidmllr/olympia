using UnityEngine;

namespace Game
{
    /// <summary>
    /// This class is used to handle an early game over by the user.
    /// It is attached to the Tilemap_Base.
    /// </summary>
    public class GameOverHandler : MonoBehaviour
    {
        private static GameManager GameManager => GameManager.Instance;
        
        /// <summary>
        /// If character enters the trigger associated with this collider,
        /// a game over mechanism is performed.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                HandleEndOfGame();
            }
        }
        
        /// <summary>
        /// Calls the end of the game in the GameManager.
        /// </summary>
        private void HandleEndOfGame()
        {
            GameManager.HandleEndOfGame(true);
        }
    }
}
