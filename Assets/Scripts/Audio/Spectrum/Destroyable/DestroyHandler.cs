using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio.Spectrum.Destroyable
{
    /// <summary>
    /// This class is used to handle the processing of note / obstacle destruction.
    /// Which sounds more cruel than it actually is.
    /// </summary>
    public class DestroyHandler : Singleton<DestroyHandler>
    {
        [HideInInspector] public List<Vector3Int> deletable;

        /// <summary>
        /// Deletable is defined.
        /// </summary>
        private void Start()
        {
            deletable = new List<Vector3Int>();
        }

        /// <summary>
        /// Checks if a note is still available and therefore allowed to be rendered.
        /// </summary>
        /// <param name="position">Position of the note to be checked</param>
        /// <returns>If a note is allowed to be rendered</returns>
        public bool IsAvailable(Vector3Int position)
        {
            return !IsMarkedForDestruction(position) && !IsDestroyedNote(position);
        }

        /// <summary>
        /// Checks if a note is marked for destruction
        /// </summary>
        /// <param name="position">Position of the note to be checked</param>
        /// <returns>If a note is marked for destruction</returns>
        public bool IsMarkedForDestruction(Vector3Int position)
        {
            return deletable.Contains(position);
        }

        /// <summary>
        /// Checks if a note is already destroyed (which means if the player triggered its collider.
        /// If so, its deletable index is updated to the current position.
        /// </summary>
        /// <param name="position">Position of the note to be checked</param>
        /// <returns>If a note was already destroyed</returns>
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
        /// Add a note to the list of destroyed notes
        /// </summary>
        /// <param name="position">The note to add</param>
        public void AddDestroyable(Vector3Int position)
        {
            deletable.Add(position);
        }
    }
}