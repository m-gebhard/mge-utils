using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace MGeLabs.Utils.Data
{
    /// <summary>
    /// A concrete implementation of <see cref="ExternalFileLoaderBase{T}"/> for loading texture files into <see cref="UnityEngine.Texture2D"/> objects.
    /// </summary>
    public class ExternalTextureLoader : ExternalFileLoaderBase<Texture2D>
    {
        /// <summary>
        /// Supported texture file extensions.
        /// </summary>
        protected override List<string> SupportedExtensions => new() { ".png", ".jpg", ".jpeg" };

        /// <summary>
        /// Loads a single texture file asynchronously.
        /// </summary>
        /// <param name="handle">The file handle representing the texture file.</param>
        /// <param name="onLoadingFinished">Callback invoked with the loaded Texture2D upon success.</param>
        /// <param name="onError">Callback invoked if the texture fails to load.</param>
        /// <returns>An enumerator for the coroutine.</returns>
        protected override IEnumerator LoadSingleFileRoutine(
            FileHandle handle,
            Action<Texture2D> onLoadingFinished,
            Action<FileHandle, UnityWebRequest.Result> onError = null
        )
        {
            string url = "file:///" + handle.FullPath.Replace("\\", "/");
            using UnityWebRequest www = UnityWebRequestTexture.GetTexture(url, false);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                texture.name = handle.FileName;

                if (!texture.isReadable)
                {
                    Texture2D readable = new(texture.width, texture.height, texture.format, false);
                    Graphics.CopyTexture(texture, readable);
                    UnityEngine.Object.Destroy(texture);
                    texture = readable;
                }

                onLoadingFinished?.Invoke(texture);
            }
            else
            {
                onError?.Invoke(handle, www.result);
            }
        }
    }
}