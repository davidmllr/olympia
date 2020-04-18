using System;
using System.Collections;
using System.IO;
using SimpleFileBrowser;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Audio
{
    /// <summary>
    /// This class handles all file operations and conversions.
    /// It makes heavy use of the SimpleFileBrowser plugin, which was downloaded from the Asset Store.
    /// </summary>
    public class FileHandler : Singleton<FileHandler>
    {
        public AudioClip clip;

        /// <summary>
        /// When class in enabled, set filters for file browsing.
        /// </summary>
        private void OnEnable()
        {
            FileBrowser.SetFilters(false, new FileBrowser.Filter("Audio", ".mp3", ".wav", ".ogg"));
        }

        /// <summary>
        /// Gets called whenever a file dialog is opened.
        /// </summary>
        /// <param name="callback">Callback method that gets called when a file was successfully picked by the user</param>
        /// <returns>A coroutine</returns>
        private IEnumerator OpenFileDialog(Action<string> callback)
        {
            yield return FileBrowser.WaitForLoadDialog(false, null, "Load Audio", "Load");

            if (FileBrowser.Success) callback(FileBrowser.Result);
        }

        /// <summary>
        /// Opens the file browser with a coroutine and calls another coroutine on success, which loads the audio to a variable.
        /// </summary>
        public void Select()
        {
            StartCoroutine(OpenFileDialog(path =>
                StartCoroutine(LoadAudio(path))));
        }

        /// <summary>
        /// Converts the given path to an AudioClip which can be assigned to an AudioSource.
        /// </summary>
        /// <param name="path">Path of the file to convert</param>
        /// <returns>A coroutine</returns>
        private IEnumerator LoadAudio(string path)
        {
            var url = $"file://{path}";
            Debug.Log($"Url is {url}");
            var type = GetAudioType(Path.GetExtension(path));
            using (var request = UnityWebRequestMultimedia.GetAudioClip(url, type))
            {
                yield return request.Send();

                if (request.isNetworkError)
                {
                    Debug.Log(request.error);
                }
                else
                {
                    clip = DownloadHandlerAudioClip.GetContent(request);
                    if (clip != null)
                        clip.name = Path.GetFileNameWithoutExtension(path) ?? throw new InvalidDataException();
                    MenuManager.Instance.UpdatePlayMenu();
                }
            }
        }

        /// <summary>
        /// Finds the appropriate AudioType for the given extension.
        /// </summary>
        /// <param name="extension">Given extension of a file</param>
        /// <returns>The associated AudioType which is used by a UnityWebRequestMultimedia later</returns>
        private AudioType GetAudioType(string extension)
        {
            switch (extension)
            {
                case "mp3":
                    return AudioType.MPEG;
                case "wav":
                    return AudioType.WAV;
                case "ogg":
                    return AudioType.OGGVORBIS;
                default:
                    return default;
            }
        }
    }
}