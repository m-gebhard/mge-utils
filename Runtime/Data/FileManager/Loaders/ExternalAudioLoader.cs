using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MGeLabs.Utils.Data
{
    /// <summary>
    /// A concrete implementation of <see cref="ExternalFileLoaderBase{T}"/> for loading audio files into <see cref="UnityEngine.AudioClip"/> objects.
    /// </summary>
    public class ExternalAudioLoader : ExternalFileLoaderBase<AudioClip>
    {
        /// <summary>
        /// Gets the list of supported audio file extensions.
        /// </summary>
        protected override List<string> SupportedExtensions => new() { ".mp3", ".wav" };

        /// <summary>
        /// Loads a single audio file asynchronously.
        /// </summary>
        /// <param name="handle">The file handle representing the audio file to load.</param>
        /// <param name="onLoaded">Callback invoked with the loaded audio clip upon success.</param>
        /// <param name="onError">Callback invoked if the audio file fails to load.</param>
        /// <returns>An enumerator for the coroutine.</returns>
        protected override IEnumerator LoadSingleFileRoutine(
            FileHandle handle,
            Action<AudioClip> onLoaded,
            Action<FileHandle, int> onError = null
        )
        {
            string url = "file:///" + handle.FullPath.Replace("\\", "/");

            AudioType type = GetAudioType(handle);
            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, type);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                audioClip.name = handle.FileName;

                onLoaded?.Invoke(audioClip);
            }
            else
            {
                onError?.Invoke(handle, (int)www.result);
            }
        }

        /// <summary>
        /// Determines the audio type based on the file extension.
        /// </summary>
        /// <param name="fileHandle">The file handle representing the audio file.</param>
        /// <returns>The corresponding <see cref="AudioType"/>.</returns>
        protected virtual AudioType GetAudioType(FileHandle fileHandle) => fileHandle switch
        {
            { Extension: ".mp3" } => AudioType.MPEG,
            { Extension: ".wav" } => AudioType.WAV,
            _ => AudioType.UNKNOWN,
        };
    }
}