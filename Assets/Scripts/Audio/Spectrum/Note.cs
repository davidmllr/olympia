using Game;
using UnityEngine;

namespace Audio.Spectrum
{
    public class Note : MonoBehaviour
    {
        public Vector3Int position;

        private NoteHandler NoteHandler => NoteHandler.Instance;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!NoteHandler.IsMarkedForDestruction(position))
            {
                GameManager.Instance.AddPoint();
                NoteHandler.AddDestroyable(position);
            }
        }
    }
}