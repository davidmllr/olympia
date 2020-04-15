using System;
using Game;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Map
{
    /// <summary>
    /// 
    /// </summary>
    public class GameOverHandler : MonoBehaviour
    {
        private static GameManager GameManager => GameManager.Instance;
        
        /// <summary>
        /// 
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
        /// 
        /// </summary>
        private void HandleEndOfGame()
        {
            GameManager.HandleEndOfGame(true);
        }
    }
}
