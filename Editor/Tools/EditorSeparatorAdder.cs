#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MGeLabs.Utils.Editor
{
    public static class EditorSeparatorAdder
    {
        /// <summary>
        /// Adds an empty GameObject to act as a separator in the Unity hierarchy.
        /// </summary>
        [MenuItem("GameObject/Separator", false, -10)]
        private static void AddSeparatorToHierarchy()
        {
            GameObject separator = new("----------------------------------");
            separator.SetActive(false);
            Undo.RegisterCreatedObjectUndo(separator, "Add Separator");

            Selection.activeGameObject = separator;
        }
    }
}
#endif