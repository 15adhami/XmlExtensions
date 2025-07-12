using UnityEngine;
using Verse;

namespace XmlExtensions.Setting
{
    internal class DrawImage : SettingContainer
    { // Add Actiom on click
        public string texPath;
        public string anchor = "Middle";
        public Vector2 dimensions = new Vector2(-1, -1);
        public float scale = -1;

        private Texture2D img;

        protected override bool Init()
        {
            img = ContentFinder<Texture2D>.Get(texPath, false);
            if (img == null)
            {
                Error("Failed to find a texture with texpath=\"" + texPath + "\"");
                return false;
            }
            return true;
        }

        protected override float CalculateHeight(float width2)
        {
            Texture2D img = ContentFinder<Texture2D>.Get(texPath);
            int height = img.height;
            int width = img.width;
            if ((dimensions.x < 0 || dimensions.y < 0) && scale < 0)
            {
                if (width > width2)
                {
                    height = (int)(height / (width / width2));
                }
            }
            else if (scale < 0)
            {
                height = (int)dimensions.y;
            }
            else
            {
                width = (int)(width * scale);
                height = (int)(height * scale);
                if (width > width2)
                {
                    height = (int)(height / (width / width2));
                }
            }
            return height;
        }

        protected override void DrawSettingContents(Rect inRect)
        {
            int height = img.height;
            int width = img.width;
            Rect drawRect = new Rect();
            if ((dimensions.x < 0 || dimensions.y < 0) && scale < 0)
            {
                if (width > inRect.width)
                {
                    height = (int)(height / (width / inRect.width));
                    width = (int)inRect.width;
                }
                Rect tempRect = inRect.TopPartPixels(height);
                if (anchor == "Middle")
                {
                    drawRect = tempRect.LeftPartPixels((tempRect.width + width) / 2);
                    drawRect = drawRect.RightPartPixels(width);
                }
                else if (anchor == "Right")
                    drawRect = tempRect.RightPartPixels(width);
                else
                    drawRect = tempRect.LeftPartPixels(width);
            }
            else if (scale < 0)
            {
                float width2 = 0;
                if (dimensions.x > inRect.width)
                {
                    width2 = inRect.width;
                }
                else
                {
                    width2 = dimensions.x;
                }
                height = (int)dimensions.y;
                Rect tempRect = inRect.TopPartPixels(height);
                if (anchor == "Middle")
                {
                    drawRect = tempRect.LeftPartPixels((tempRect.width + width2) / 2);
                    drawRect = drawRect.RightPartPixels(width2);
                }
                else if (anchor == "Right")
                    drawRect = tempRect.RightPartPixels(width2);
                else
                    drawRect = tempRect.LeftPartPixels(width2);
            }
            else
            {
                width = (int)(width * scale);
                height = (int)(height * scale);
                if (width > inRect.width)
                {
                    height = (int)(height / (width / inRect.width));
                    width = (int)inRect.width;
                }
                Rect tempRect = inRect.TopPartPixels(height);
                if (anchor == "Middle")
                {
                    drawRect = tempRect.LeftPartPixels((tempRect.width + width) / 2);
                    drawRect = drawRect.RightPartPixels(width);
                }
                else if (anchor == "Right")
                    drawRect = tempRect.RightPartPixels(width);
                else
                    drawRect = tempRect.LeftPartPixels(width);
            }
            GUI.DrawTexture(drawRect, img);
        }
    }
}