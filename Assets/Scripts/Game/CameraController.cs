using UnityEngine;

namespace Game
{
    /// <summary>
    /// This class was build to handle some camera related logic.
    /// </summary>
    public class CameraController : Singleton<CameraController>
    {
        public Camera mainCamera;
        public Camera miniCamera;

        /// <summary>
        /// Play an animation for the main camera when game is started.
        /// </summary>
        public void Start()
        {
            mainCamera.GetComponent<Animation>().Play();
        }
        
    }
}