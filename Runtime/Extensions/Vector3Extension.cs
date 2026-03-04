using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MGeLabs.Utils.Extensions
{
    /// <summary>
    /// Provides extension methods for the Vector3 class.
    /// </summary>
    public static class Vector3Extension
    {
        /// <summary>
        /// Creates a new vector with the specified components replaced by the provided values.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="x">The new x-component value, or null to keep the original x-component.</param>
        /// <param name="y">The new y-component value, or null to keep the original y-component.</param>
        /// <param name="z">The new z-component value, or null to keep the original z-component.</param>
        /// <returns>A new vector with the specified components replaced by the provided values.</returns>
        /// <example>
        /// <code>
        /// Vector3 v = new Vector3(1, 2, 3);
        /// Vector3 modified = v.With(y: 10);  // Result: (1, 10, 3)
        /// </code>
        /// </example>
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(
                x ?? vector.x,
                y ?? vector.y,
                z ?? vector.z
            );
        }

        /// <summary>
        /// Multiplies each component of the vector by the corresponding component of another vector.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="other">The vector to multiply with.</param>
        /// <returns>A new vector with each component multiplied by the corresponding component of the other vector.</returns>
        /// <example>
        /// <code>
        /// Vector3 a = new Vector3(2, 3, 4);
        /// Vector3 b = new Vector3(5, 6, 7);
        /// Vector3 result = a.Multiply(b);  // Result: (10, 18, 28)
        /// </code>
        /// </example>
        public static Vector3 Multiply(this Vector3 vector, Vector3 other)
        {
            return new Vector3(vector.x * other.x, vector.y * other.y, vector.z * other.z);
        }

        /// <summary>
        /// Multiplies each component of the vector by a scalar value.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="value">The scalar value to multiply with.</param>
        /// <returns>A new vector with each component multiplied by the scalar value.</returns>
        /// <example>
        /// <code>
        /// Vector3 v = new Vector3(1, 2, 3);
        /// Vector3 result = v.Multiply(2);  // Result: (2, 4, 6)
        /// </code>
        /// </example>
        public static Vector3 Multiply(this Vector3 vector, float value)
        {
            return new Vector3(vector.x * value, vector.y * value, vector.z * value);
        }

        /// <summary>
        /// Divides each component of the vector by the corresponding component of another vector.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="other">The vector to divide by.</param>
        /// <returns>A new vector with each component divided by the corresponding component of the other vector, or the original component if the divisor is zero.</returns>
        /// <example>
        /// <code>
        /// Vector3 a = new Vector3(10, 20, 30);
        /// Vector3 b = new Vector3(2, 0, 5);
        /// Vector3 result = a.Divide(b);  // Result: (5, 20, 6) — y unchanged because divisor is 0
        /// </code>
        /// </example>
        public static Vector3 Divide(this Vector3 vector, Vector3 other)
        {
            return new Vector3(
                other.x != 0 ? vector.x / other.x : vector.x,
                other.y != 0 ? vector.y / other.y : vector.y,
                other.z != 0 ? vector.z / other.z : vector.z
            );
        }

        /// <summary>
        /// Divides each component of the vector by a scalar value.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="value">The scalar value to divide by.</param>
        /// <returns>A new vector with each component divided by the scalar value, or the original component if the divisor is zero.</returns>
        /// <example>
        /// <code>
        /// Vector3 v = new Vector3(10, 20, 30);
        /// Vector3 result = v.Divide(2);  // Result: (5, 10, 15)
        /// </code>
        /// </example>
        public static Vector3 Divide(this Vector3 vector, float value)
        {
            bool isZero = value == 0;
            return new Vector3(
                !isZero ? vector.x / value : vector.x,
                !isZero ? vector.y / value : vector.y,
                !isZero ? vector.z / value : vector.z
            );
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="other">The vector to calculate the distance to.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <example>
        /// <code>
        /// Vector3 a = new Vector3(0, 0, 0);
        /// Vector3 b = new Vector3(3, 4, 0);
        /// float dist = a.DistanceTo(b);  // Result: 5
        /// </code>
        /// </example>
        public static float DistanceTo(this Vector3 vector, Vector3 other)
        {
            return Vector3.Distance(vector, other);
        }

        /// <summary>
        /// Draws a downward-pointing gizmo arrow at the given world position (Editor only).
        /// </summary>
        /// <param name="position">World-space position where the arrow tip is placed (arrow points down to this position).</param>
        /// <param name="label">Optional label text shown above the arrow start. Empty string to omit.</param>
        /// <param name="color">Optional color for the arrow and label. If null, defaults to <see cref="Color.green"/>.</param>
        /// <param name="length">Distance from the arrow start (top) down to the tip (position).</param>
        /// <param name="angle">Angle (degrees) used to form the arrowhead relative to the shaft.</param>
        /// <param name="fontSize">Font size for the optional label.</param>
        /// <param name="drawAdditionalDisk">Whether to draw an additional wire disc at the arrow tip for better visibility.</param>
        public static void DrawGizmosDownArrow(
            this Vector3 position,
            string label = "",
            Color? color = null,
            float length = 10f,
            float angle = 20f,
            int fontSize = 16,
            bool drawAdditionalDisk = false)
        {
#if UNITY_EDITOR
            float headLength = Mathf.Clamp(HandleUtility.GetHandleSize(position), 0.25f, 2f);

            Vector3 dir = Vector3.down;
            Vector3 start = position + Vector3.up * length;
            Vector3 end = position;

            Gizmos.color = color ?? Color.green;

            Gizmos.DrawLine(start, end);
            Vector3 right = Quaternion.AngleAxis(angle, Vector3.forward) * -dir;
            Vector3 left = Quaternion.AngleAxis(-angle, Vector3.forward) * -dir;

            Gizmos.DrawLine(end, end + right * headLength);
            Gizmos.DrawLine(end, end + left * headLength);

            if (drawAdditionalDisk)
            {
                Handles.color = color ?? Color.green;
                Handles.DrawWireDisc(position, Vector3.up, headLength * 0.5f);
            }

            if (!string.IsNullOrEmpty(label))
            {
                GUIStyle style = new(EditorStyles.boldLabel);
                style.normal.textColor = Gizmos.color;
                style.fontSize = fontSize;
                Handles.Label(start + Vector3.up * 0.1f, label, style);
            }
#endif
        }
    }
}