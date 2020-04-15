using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OverlayController : Singleton<OverlayController>
    {
        [SerializeField] private Text score;
        [SerializeField] private Transform time;

        private Text timeCurrent => time.Find("Current").GetComponent<Text>();
        private Text timeTotal => time.Find("Total").GetComponent<Text>();

        /// <summary>
        /// </summary>
        /// <param name="points"></param>
        public void SetScore(long points)
        {
            score.text = points.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds"></param>
        public void SetTimeCurrent(float seconds)
        {
            var span = TimeSpan.FromSeconds(seconds);
            timeCurrent.text = span.ToString(@"mm\:ss");
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds"></param>
        public void SetTimeTotal(float seconds)
        {
            var span = TimeSpan.FromSeconds(seconds);
            timeTotal.text = span.ToString(@"mm\:ss");
        }
        
    }
}