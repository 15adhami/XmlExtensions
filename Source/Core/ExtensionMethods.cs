using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace XmlExtensions
{
    internal static class ExtensionMethods
    {
        extension(Rect rect)
        {
            internal Rect MiddlePartPixels(float width, float height)
            {
                return rect.LeftPartPixels(width + 0.5f * (rect.width - width)).RightPartPixels(width);
            }
        }
    }
}
