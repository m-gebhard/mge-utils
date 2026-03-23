using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MGeLabs.Utils.Editor
{
    /// <summary>
    /// Attribute that marks a field as read-only in the Unity Inspector.
    /// Apply this attribute to a field to render it disabled (non-editable) in the editor.
    /// </summary>
    public class ReadOnlyFieldAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
    /// <summary>
    /// Custom property drawer for <see cref="ReadOnlyFieldAttribute"/>.
    /// Renders the target property as disabled in the Inspector so it cannot be edited.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyFieldAttribute))]
    public class ReadOnlyFieldDrawer : PropertyDrawer
    {
        /// <summary>
        /// Draws the property GUI with editing disabled, then restores the GUI.enabled state.
        /// </summary>
        /// <param name="position">Screen rect for the control.</param>
        /// <param name="property">The serialized property to draw.</param>
        /// <param name="label">Label of the property field.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool wasEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = wasEnabled;
        }
    }
#endif
}