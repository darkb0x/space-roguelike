using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class CombineSprites
    {
        // Данный код тяжёлый для работы в рантайме.
        public static Sprite MergeSprites(Sprite[] spritesToMerge, Vector2Int size, string name = "New Sprite")
        {
            Resources.UnloadUnusedAssets();

            var newTex = new Texture2D(size.x, size.y);

            for (int x = 0; x < newTex.width; x++)
            {
                for (int y = 0; y < newTex.height; y++)
                {
                    newTex.SetPixel(x, y, new Color(1, 1, 1, 0));
                }
            }

            for (int i = 0; i < spritesToMerge.Length; i++)
            {
                for (int x = 0; x < spritesToMerge[i].texture.width; x++)
                {
                    for (int y = 0; y < spritesToMerge[i].texture.height; y++)
                    {
                        var color = spritesToMerge[i].texture.GetPixel(x, y).a == 0 ? newTex.GetPixel(x, y) : spritesToMerge[i].texture.GetPixel(x, y);

                        newTex.SetPixel(x, y, color);
                    }
                }
            }

            newTex.filterMode = FilterMode.Point;
            newTex.Apply();

            Sprite finalSprite = Sprite.Create(newTex, new Rect(0, 0, size.x, size.y), new Vector2(0.5f, 0.5f), 16, 0, SpriteMeshType.Tight);
            finalSprite.name = name;
            return finalSprite;
        }
    }
}
