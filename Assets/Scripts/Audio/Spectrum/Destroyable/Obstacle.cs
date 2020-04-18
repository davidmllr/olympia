using Game;
using UnityEngine;

namespace Audio.Spectrum.Destroyable
{
    /// <summary>
    /// Represents an obstacle.
    /// </summary>
    public class Obstacle : MonoBehaviour
    {
        public Vector3Int position;

        private DestroyHandler DestroyHandler => DestroyHandler.Instance;

        /// <summary>
        /// When the collider of the obstacle is triggered, a point is removed in the GameManager.
        /// Moreover, it is registered for destruction in the next frame.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!DestroyHandler.IsMarkedForDestruction(position))
            {
                GameManager.Instance.RemovePoint();
                DestroyHandler.AddDestroyable(position);
            }
        }
    }
}