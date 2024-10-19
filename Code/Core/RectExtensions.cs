using UnityEngine;

namespace Enigmatic.Core
{
    public static class RectExtensions
    {
        /// <summary>
        /// Scale Rect arount top left pivod
        /// </summary>
        public static Rect ScaleSize(this Rect rect, float scale)
        {
            return rect.ScaleSize(scale, rect.TopLeft());
        }

        /// <summary>
        /// Sacle rect arount pivot
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scale"></param>
        /// <param name="pivotPosition"></param>
        /// <returns></returns>
        public static Rect ScaleSize(this Rect rect, float scale, Vector2 pivotPosition)
        {
            Rect result = rect;

            result.x += pivotPosition.x;
            result.y += pivotPosition.y;

            result.xMin *= scale;
            result.xMax *= scale;

            result.yMin *= scale;
            result.yMax *= scale;

            result.x += pivotPosition.x;
            result.y += pivotPosition.y;

            return result;
        }

        public static Vector2 TopLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin);
        }

        public static bool Overlap(this Rect rect1, Rect rect2)
        {
            if (rect1.x + rect1.width < rect2.x || rect2.x + rect2.width < rect1.x
                || rect1.y + rect1.height < rect2.y || rect2.y + rect2.height < rect1.y)
            {
                return false;
            }

            return true;
        }

        public static Vector2 Center(this Rect rect)
        {
            return rect.position + rect.size / 2;
        }
    }
}
