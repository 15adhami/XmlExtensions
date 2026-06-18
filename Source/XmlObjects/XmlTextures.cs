using System;
using UnityEngine;
using Verse;

namespace XmlExtensions
{
    [StaticConstructorOnStartup]
    internal static class XmlTextures
    {
        private static Texture2D revealOpenTexture;

        internal static Texture2D RevealOpenTexture
        {
            get
            {
                if (revealOpenTexture == null)
                {
                    revealOpenTexture = RotatedClockwise(TexButton.Reveal);
                }

                return revealOpenTexture;
            }
        }

        static XmlTextures()
        {
            // Intentionally empty.
            // RevealOpenTexture is generated lazily because it requires a graphics device.
        }

        private static Texture2D RotatedClockwise(Texture texture)
        {
            RenderTexture previous = RenderTexture.active;

            RenderTexture rt = RenderTexture.GetTemporary(
                texture.width,
                texture.height,
                0,
                RenderTextureFormat.ARGB32
            );

            Graphics.Blit(texture, rt);
            RenderTexture.active = rt;

            Texture2D source = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);
            source.ReadPixels(new Rect(0f, 0f, texture.width, texture.height), 0, 0);
            source.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);

            Texture2D rotated = new Texture2D(source.height, source.width, TextureFormat.ARGB32, false);

            Color[] pixels = source.GetPixels();
            Color[] rotatedPixels = new Color[pixels.Length];

            int width = source.width;
            int height = source.height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    rotatedPixels[(width - x - 1) * height + y] = pixels[y * width + x];
                }
            }

            rotated.SetPixels(rotatedPixels);
            rotated.Apply();

            UnityEngine.Object.Destroy(source);
            return rotated;
        }
    }
}
