#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MGeLabs.Utils.Editor
{
    public static class EditorSeparatorAdder
    {
        /// <summary>
        /// Adds an empty GameObject to act as a separator in the Unity hierarchy.
        /// If an object is selected, the separator will be added as a sibling below the selected object; otherwise, it will be added at the root level.
        /// </summary>
        [MenuItem("GameObject/Separator", false, -10)]
        private static void AddSeparatorToHierarchy()
        {
            GameObject separator = new("----------------------------------");
            separator.SetActive(false);

            GameObject selected = Selection.activeGameObject;
            if (selected != null)
            {
                separator.transform.SetParent(selected.transform.parent, false);
                separator.transform.SetSiblingIndex(selected.transform.GetSiblingIndex() + 1);
            }

            Undo.RegisterCreatedObjectUndo(separator, "Add Separator");
            Selection.activeGameObject = separator;
        }
    }
}
#endif