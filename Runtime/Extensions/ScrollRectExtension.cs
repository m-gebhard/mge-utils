using System.Collections;
using UnityEngine.UI;

namespace MGeLabs.Utils.Extensions
{
    /// <summary>
    /// Provides extension methods for ScrollRects.
    /// </summary>
    public static class ScrollRectExtension
    {
        /// <summary>
        /// Resets the scroll position of a ScrollRect to the specified value (default is 1, which corresponds to the left/top).
        /// </summary>
        /// <param name="scrollRect">The ScrollRect component whose scroll position will be reset.</param>
        /// <param name="scroll">The normalized scroll position to set (0 = right/bottom, 1 = left/top). Default is 1.</param>
        /// <param name="direction">The scroll direction to reset: 0 = vertical, 1 = horizontal, -1 = both. Default is -1 (both).</param>
        /// <returns>>An IEnumerator to be used as a coroutine.</returns>
        public static IEnumerator ResetContentScroll(this ScrollRect scrollRect, float scroll = 1f, int direction = -1)
        {
            yield return null;

            switch (direction)
            {
                case 0:
                    scrollRect.verticalNormalizedPosition = scroll;
                    break;
                case 1:
                    scrollRect.horizontalNormalizedPosition = scroll;
                    break;
                default:
                    scrollRect.verticalNormalizedPosition = scroll;
                    scrollRect.horizontalNormalizedPosition = scroll;
                    break;
            }
        }
    }
}