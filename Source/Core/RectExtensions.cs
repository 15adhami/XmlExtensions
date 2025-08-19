using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                return rect.RightPart(1f - pct);
            }

            internal Rect TrimLeftPartPixels(float pixels)
            {
                return rect.RightPartPixels(rect.width - pixels);
            }

            internal Rect TrimRightPartPixels(float pixels)
            {
                return rect.RightPartPixels(rect.width - pixels);
            }
        }
    }
}
