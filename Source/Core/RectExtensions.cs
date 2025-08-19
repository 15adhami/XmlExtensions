using UnityEngine;
using Verse;

namespace XmlExtensions
{
    internal static class RectExtensions
    {
        extension(Rect rect)
        {
            internal Rect TrimLeftPart(float pct)
            {
                return rect.RightPart(1f - pct);
            }

            internal Rect TrimRightPart(float pct)
            {
                return rect.LeftPart(1f - pct);
            }

            internal Rect TrimTopPart(float pct)
            {
                return rect.BottomPart(1f - pct);
            }

            internal Rect TrimBottomPart(float pct)
            {
                return rect.TopPart(1f - pct);
            }

            internal Rect TrimLeftPartPixels(float pixels)
            {
                return rect.RightPartPixels(rect.width - pixels);
            }

            internal Rect TrimRightPartPixels(float pixels)
            {
                return rect.LeftPartPixels(rect.width - pixels);
            }

            internal Rect TrimTopPartPixels(float pixels)
            {
                return rect.BottomPartPixels(rect.height - pixels);
            }

            internal Rect TrimBottomPartPixels(float pixels)
            {
                return rect.TopPartPixels(rect.height - pixels);
            }
        }
    }
}
