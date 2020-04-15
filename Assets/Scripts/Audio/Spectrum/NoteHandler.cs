using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio.Spectrum
{
    /// <summary>
    /// </summary>
    public class NoteHandler : Singleton<NoteHandler>
    {
        [HideInInspector] public List<Vector3Int> deletable;

        /// <summary>
        /// </summary>
        private void Start()
        {
            deletable = new List<Vector3Int>();
        }

        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsAvailable(Vector3Int position)
        {
            return !IsMarkedForDestruction(position) && !IsDestroyedNote(position);
        }

        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool IsMarkedForDestruction(Vector3Int position)
        {
            return deletable.Contains(position);
        }

        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool IsDestroyedNote(Vector3Int position)
        {
            var note = deletable.FirstOrDefault(p => p.x - 1 == position.x && p.y == position.y);

            if (note == default) return false;

            var index = deletable.IndexOf(note);
            if (index == -1) return false;

            var newPos = new Vector3Int(note.x - 1, note.y, note.z);

            if (note.x <= -39)
                deletable.RemoveAt(index);
            else deletable[index] = newPos;
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        public void AddDestroyable(Vector3Int position)
        {
            deletable.Add(position);
        }
    }
}