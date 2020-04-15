using UnityEngine;

namespace Game
{
    public class CameraController : Singleton<CameraController>
    {
        public Camera mainCamera;
        public Camera miniCamera;

        /// <summary>
        /// </summary>
        public void Start()
        {
            mainCamera.GetComponent<Animation>().Play();
        }
        
    }
}