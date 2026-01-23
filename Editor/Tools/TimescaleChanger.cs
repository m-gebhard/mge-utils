#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MGeLabs.Utils.Editor
{
    /// <summary>
    /// Provides a Unity Editor window for changing the game's timescale in real-time.
    /// </summary>
    public class TimescaleChanger : EditorWindow
    {
        /// <summary>
        /// The current timescale value.
        /// </summary>
        private float timeScale = 1f;

        /// <summary>
        /// Opens the timescale Changer window from the Unity Editor menu.
        /// </summary>
        [MenuItem("Tools/MGe Labs/Timescale Changer")]
        public static void ShowWindow()
        {
            GetWindow<TimescaleChanger>("Timescale Changer");
        }

        /// <summary>
        /// Called when the window is enabled. Initializes the timescale and sets the window size.
        /// </summary>
        private void OnEnable()
        {
            timeScale = Time.timeScale;

            minSize = new Vector2(240, 100);
            maxSize = new Vector2(400, 160);

            EditorApplication.update += Repaint;
        }

        /// <summary>
        /// Called when the window is disabled. Resets the timescale and removes the repaint callback.
        /// </summary>
        private void OnDisable()
        {
            EditorApplication.update -= Repaint;
            Time.timeScale = 1f;
        }

        /// <summary>
        /// Draws the GUI for the timescale Changer window.
        /// </summary>
        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            timeScale = EditorGUILayout.Slider(timeScale, 0f, 1.5f);
            if (EditorGUI.EndChangeCheck())
            {
                ApplyTimeScale(timeScale);
            }

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            DrawPresetButton(0.25f);
            DrawPresetButton(0.5f);
            DrawPresetButton(0.75f);
            DrawPresetButton(1f);
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.Label($"Current: {Time.timeScale:0.00}x");
        }

        /// <summary>
        /// Draws a button for a specific timescale preset.
        /// </summary>
        /// <param name="value">The timescale value for the preset.</param>
        private void DrawPresetButton(float value)
        {
            if (GUILayout.Button($"{value:0.##}x"))
            {
                ApplyTimeScale(value);
            }
        }

        /// <summary>
        /// Applies the specified timescale value.
        /// </summary>
        /// <param name="value">The new timescale value to apply.</param>
        private void ApplyTimeScale(float value)
        {
            timeScale = value;
            Time.timeScale = value;
        }
    }
}
#endif