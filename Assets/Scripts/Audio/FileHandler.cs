using System;
using System.Collections;
using System.IO;
using SimpleFileBrowser;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Audio
{
    public class FileHandler : Singleton<FileHandler>
    {
        public AudioClip clip;

        /// <summary>
        /// </summary>
        private void OnEnable()
        {
            FileBrowser.SetFilters(false, new FileBrowser.Filter("Audio", ".mp3", ".wav"));
        }

        /// <summary>
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator OpenFileDialog(Action<string> callback)
        {
            yield return FileBrowser.WaitForLoadDialog(false, null, "Load Audio", "Load");

            if (FileBrowser.Success) callback(FileBrowser.Result);
        }

        /// <summary>
        /// </summary>
        public void Select()
        {
            StartCoroutine(OpenFileDialog(path =>
                StartCoroutine(LoadAudio(path))));
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
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