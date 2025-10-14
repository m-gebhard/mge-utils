#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MGeLabs.Utils.Editor
{
    /// <summary>
    /// Provides a Unity Editor window for configuring animation settings in bulk for FBX files.
    /// </summary>
    public class BulkAnimationConfigurator : EditorWindow
    {
        /// <summary>
        /// The target folder containing the FBX files to configure.
        /// </summary>
        private string targetFolder = "Assets";

        /// <summary>
        /// The reference avatar to apply to the FBX files.
        /// </summary>
        private Avatar referenceAvatar;

        /// <summary>
        /// The animation type to set for the FBX files.
        /// </summary>
        private ModelImporterAnimationType animationType = ModelImporterAnimationType.Human;

        /// <summary>
        /// Indicates whether to rename animation clips in the FBX files.
        /// </summary>
        private bool shouldRenameClips = true;

        /// <summary>
        /// The format to use when renaming animation clips.
        /// </summary>
        private string renameFormat = "{fileName}";

        /// <summary>
        /// Indicates whether to enable looping for the animation clips.
        /// </summary>
        private bool shouldLoopTime = true;

        /// <summary>
        /// Indicates whether to enable translation degrees of freedom (DoF) for the animations.
        /// </summary>
        private bool hasTranslationDoF;

        /// <summary>
        /// Indicates whether to keep the original orientation of the root transform.
        /// </summary>
        private bool keepOriginalOrientation = true;

        /// <summary>
        /// Indicates whether to keep the original position (XZ) of the root transform.
        /// </summary>
        private bool keepOriginalPositionXZ = true;

        /// <summary>
        /// Indicates whether to keep the original position (Y) of the root transform.
        /// </summary>
        private bool keepOriginalPositionY = true;

        /// <summary>
        /// Indicates whether to lock the root rotation.
        /// </summary>
        private bool lockRootRotation;

        /// <summary>
        /// Indicates whether to lock the root position (XZ).
        /// </summary>
        private bool lockRootPositionXZ;

        /// <summary>
        /// Indicates whether to lock the root height (Y).
        /// </summary>
        private bool lockRootHeightY;

        /// <summary>
        /// Displays the Bulk Animation Configurator window in the Unity Editor.
        /// </summary>
        [MenuItem("Tools/MGe Labs/Bulk Animation Configurator")]
        public static void ShowWindow()
        {
            GetWindow<BulkAnimationConfigurator>("Bulk Animation Configurator");
        }

        /// <summary>
        /// Draws the GUI for the Bulk Animation Configurator window.
        /// </summary>
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Target Folder");
            targetFolder = EditorGUILayout.TextField(targetFolder, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string selected = EditorUtility.OpenFolderPanel("Select Target Folder", targetFolder, "");
                if (!string.IsNullOrEmpty(selected))
                {
                    if (selected.StartsWith(Application.dataPath))
                        targetFolder = "Assets" + selected[Application.dataPath.Length..];
                    else
                        targetFolder = selected;
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);

            GUILayout.Label("Animation Settings", EditorStyles.boldLabel);
            referenceAvatar =
                (Avatar)EditorGUILayout.ObjectField("Reference Avatar", referenceAvatar, typeof(Avatar), false);
            animationType = (ModelImporterAnimationType)EditorGUILayout.EnumPopup("Animation Type", animationType);
            EditorGUILayout.Space(5);
            shouldRenameClips = EditorGUILayout.ToggleLeft("Rename Clips", shouldRenameClips);
            if (shouldRenameClips)
            {
                EditorGUI.indentLevel++;
                renameFormat = EditorGUILayout.TextField("Rename Format", renameFormat);
                EditorGUILayout.LabelField(
                    "{fileName} - Filename, {clipName} - Original Clip Name, {clipIndex} - Clip Index",
                    EditorStyles.miniLabel);
                EditorGUI.indentLevel--;
                EditorGUILayout.Space(5);
            }

            shouldLoopTime = EditorGUILayout.ToggleLeft("Loop Time", shouldLoopTime);
            hasTranslationDoF = EditorGUILayout.ToggleLeft("Enable Translation DoF", hasTranslationDoF);

            EditorGUILayout.Space(10);

            GUILayout.Label("Root Transform Rotation", EditorStyles.boldLabel);
            lockRootRotation = EditorGUILayout.ToggleLeft("Bake Into Pose", lockRootRotation);
            keepOriginalOrientation = EditorGUILayout.ToggleLeft("Keep Original Rotation", keepOriginalOrientation);
            EditorGUILayout.Space(5);
            GUILayout.Label("Root Transform Position (Y)", EditorStyles.boldLabel);
            lockRootHeightY = EditorGUILayout.ToggleLeft("Bake Into Pose", lockRootHeightY);
            keepOriginalPositionY = EditorGUILayout.ToggleLeft("Keep Original Position Y", keepOriginalPositionY);
            EditorGUILayout.Space(5);
            GUILayout.Label("Root Transform Position (XZ)", EditorStyles.boldLabel);
            lockRootPositionXZ = EditorGUILayout.ToggleLeft("Bake Into Pose", lockRootPositionXZ);
            keepOriginalPositionXZ = EditorGUILayout.ToggleLeft("Keep Original Position XZ", keepOriginalPositionXZ);

            EditorGUILayout.Space(10);

            if (GUILayout.Button("Apply Settings to Folder"))
            {
                ApplyToFolder(targetFolder);
            }
        }

        /// <summary>
        /// Applies the specified animation settings to all FBX files in the target folder.
        /// </summary>
        /// <param name="folder">The folder containing the FBX files to configure.</param>
        private void ApplyToFolder(string folder)
        {
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder.Replace("Assets", Application.dataPath)))
            {
                EditorUtility.DisplayDialog("Error", "Folder not found: " + folder, "OK");
                return;
            }

            string[] fbxFiles = Directory.GetFiles(folder, "*.fbx", SearchOption.AllDirectories);
            if (fbxFiles.Length == 0)
            {
                EditorUtility.DisplayDialog("No Files", "No FBX files found in " + folder, "OK");
                return;
            }

            if (folder == "Assets")
            {
                bool proceed = EditorUtility.DisplayDialog(
                    "Warning",
                    "You selected the root 'Assets' folder. This will process ALL FBX files in your project. Are you sure?",
                    "Proceed",
                    "Cancel"
                );
                if (!proceed) return;
            }

            List<string> processedFileNames = new();
            foreach (string filePath in fbxFiles)
            {
                string relativePath = filePath.Replace(Application.dataPath, "Assets");
                ProcessAnimationFile(relativePath);
                processedFileNames.Add(Path.GetFileName(relativePath));
            }

            string filesString = string.Join("\n", processedFileNames);
            Debug.Log($"Processed {processedFileNames.Count} FBX file(s) in folder: {folder}.\n{filesString}");
        }

        /// <summary>
        /// Processes a single FBX file and applies the animation settings.
        /// </summary>
        /// <param name="relativePath">The relative path of the FBX file.</param>
        private void ProcessAnimationFile(string relativePath)
        {
            ModelImporter importer = AssetImporter.GetAtPath(relativePath) as ModelImporter;
            if (importer == null) return;

            importer.animationType = animationType;
            importer.sourceAvatar = referenceAvatar;

            HumanDescription desc = importer.humanDescription;
            desc.hasTranslationDoF = hasTranslationDoF;
            importer.humanDescription = desc;

            ModelImporterClipAnimation[] clips = importer.defaultClipAnimations;
            if (clips == null) return;

            string fbxName = Path.GetFileNameWithoutExtension(relativePath);
            for (int i = 0; i < clips.Length; i++)
            {
                ModelImporterClipAnimation clip = clips[i];

                if (shouldRenameClips)
                {
                    clip.name = renameFormat
                        .Replace("{fileName}", fbxName)
                        .Replace("{clipName}", clip.name)
                        .Replace("{clipIndex}", i.ToString());
                }

                clip.loopTime = shouldLoopTime;
                clip.keepOriginalOrientation = keepOriginalOrientation;
                clip.keepOriginalPositionXZ = keepOriginalPositionXZ;
                clip.keepOriginalPositionY = keepOriginalPositionY;
                clip.lockRootRotation = lockRootRotation;
                clip.lockRootPositionXZ = lockRootPositionXZ;
                clip.lockRootHeightY = lockRootHeightY;
            }

            importer.clipAnimations = clips;

            importer.SaveAndReimport();
        }
    }
}
#endif