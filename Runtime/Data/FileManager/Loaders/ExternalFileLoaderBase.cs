using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MGeLabs.Utils.Extensions;
using UnityEngine.Networking;

namespace MGeLabs.Utils.Data
{
    /// <summary>
    /// Abstract base class for loading external files of type <typeparamref name="T"/>.
    /// Can be extended to support different file types by implementing the abstract methods.
    /// </summary>
    /// <typeparam name="T">The type of data to be returned (AudioClip, VideoClip, ...).</typeparam>
    public abstract class ExternalFileLoaderBase<T>
    {
        /// <summary>
        /// Gets the list of supported file extensions for this loader.
        /// </summary>
        protected abstract List<string> SupportedExtensions { get; }

        /// <summary>
        /// Loads files from a specified directory asynchronously in batches.
        /// </summary>
        /// <param name="relativePath">The relative path to the directory containing the files.</param>
        /// <param name="location">The storage location of the directory (default is DataPath).</param>
        /// <param name="onLoadingFinished">
        /// Callback invoked when all files have been loaded successfully; receives the list of loaded items.
        /// </param>
        /// <param name="onBatchFinished">
        /// Callback invoked after each batch of files is loaded; receives the full list of currently loaded items.
        /// </param>
        /// <param name="onError">
        /// Callback invoked for each file that fails to load; receives the file handle and the error result.
        /// </param>
        /// <param name="loadBatchSize">The number of files to load per batch (default is 10).</param>
        /// <returns>An enumerator for the coroutine.</returns>
        public virtual IEnumerator LoadFromDirectory(
            string relativePath,
            EStorageLocation location = EStorageLocation.DataPath,
            Action<IReadOnlyList<T>> onLoadingFinished = null,
            Action<IReadOnlyList<T>> onBatchFinished = null,
            Action<FileHandle, UnityWebRequest.Result> onError = null,
            int loadBatchSize = 10
        )
        {
            List<T> loadedItems = new List<T>();
            List<FileHandle> files = FindFilesInDirectory(relativePath, location);

            yield return files.ProcessInBatches(
                file => LoadSingleFileRoutine(file, loadedItems.Add, onError),
                itemsPerBatch: loadBatchSize,
                onFinished: (_) => onLoadingFinished?.Invoke(loadedItems),
                onBatchFinished: (_) => onBatchFinished?.Invoke(loadedItems)
            );
        }

        /// <summary>
        /// Finds files in the specified directory that match the supported extensions.
        /// </summary>
        /// <param name="relativePath">The relative path to the directory.</param>
        /// <param name="location">The storage location of the directory (default is DataPath).</param>
        /// <returns>A list of file handles for the matching files.</returns>
        protected virtual List<FileHandle> FindFilesInDirectory(
            string relativePath,
            EStorageLocation location = EStorageLocation.DataPath
        )
        {
            return FileManager.GetFilesInDirectory(relativePath, location)
                .Where(f => SupportedExtensions.Contains(f.Extension))
                .ToList();
        }

        /// <summary>
        /// Loads a single file asynchronously.
        /// Must be implemented by derived classes to handle the loading/parsing of the specific file type.
        /// </summary>
        /// <param name="handle">The file handle representing the file to load.</param>
        /// <param name="onLoadingFinished">Callback invoked with the loaded item upon success.</param>
        /// <param name="onError">Callback invoked if the file fails to load.</param>
        /// <returns>An enumerator for the coroutine.</returns>
        protected abstract IEnumerator LoadSingleFileRoutine(
            FileHandle handle,
            Action<T> onLoadingFinished,
            Action<FileHandle, UnityWebRequest.Result> onError = null
        );
    }
}