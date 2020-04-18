using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// This class is used to handle UI elements in the overlay within the game.
    /// </summary>
    public class OverlayController : Singleton<OverlayController>
    {
        [SerializeField] private Text score;
        [SerializeField] private Transform time;

        private Text timeCurrent => time.Find("Current").GetComponent<Text>();
        private Text timeTotal => time.Find("Total").GetComponent<Text>();

        /// <summary>
        /// Sets the score in the overlay.
        /// </summary>
        /// <param name="points">Current points</param>
        public void SetScore(long points)
        {
            score.text = points.ToString();
        }

        /// <summary>
        /// Sets the current time in the overlay.
        /// </summary>
        /// <param name="seconds">Current time in seconds</param>
        public void SetTimeCurrent(float seconds)
        {
            var span = TimeSpan.FromSeconds(seconds);
            timeCurrent.text = span.ToString(@"mm\:ss");
        }
        
        /// <summary>
        /// Sets the total time in the overlay.
        /// </summary>
        /// <param name="seconds">The total time in seconds</param>
        public void SetTimeTotal(float seconds)
        {
            var span = TimeSpan.FromSeconds(seconds);
            timeTotal.text = span.ToString(@"mm\:ss");
        }
        
    }
}