using Game;
using UnityEngine;

namespace Audio.Spectrum.Destroyable
{
    /// <summary>
    /// Represents a note (which represents a point).
    /// </summary>
    public class Note : MonoBehaviour
    {
        public Vector3Int position;

        private DestroyHandler DestroyHandler => DestroyHandler.Instance;

        /// <summary>
        /// When the collider of the note is triggered, a point is added in the GameManager.
        /// Moreover, it is registered for destruction in the next frame.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!DestroyHandler.IsMarkedForDestruction(position))
            {
                GameManager.Instance.AddPoint();
                DestroyHandler.AddDestroyable(position);
            }
        }
    }
}