using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    public class PickedGameObj : MonoBehaviour
    {
        [System.Serializable]
        public struct Sprite
        {
            public SpriteRenderer render;
            [NaughtyAttributes.SortingLayer] public string sortingLayer;

            public Sprite(SpriteRenderer r, string l)
            {
                render = r;
                sortingLayer = l;
            }
        }

        [SerializeField, NaughtyAttributes.ReadOnly] List<Sprite> sprites = new List<Sprite>();
        [SerializeField] private List<Sprite> spriteExceptions = new List<Sprite>();

        private void SetSortingLayer(string layer)
        {
            foreach (var sprite in sprites)
            {
                sprite.render.sortingLayerName = layer;
            }
        }
        private void ResetSortingLayer()
        {
            foreach (var sprite in sprites)
            {
                if(spriteExceptions.Contains(sprite))
                {
                    sprite.render.sortingLayerName = spriteExceptions[spriteExceptions.IndexOf(sprite)].sortingLayer;
                    continue;
                }

                sprite.render.sortingLayerName = sprite.sortingLayer;
            }
        }
        public void SetSpritesColor(Color color, string sortingLayer = null)
        {
            if(sprites == null | sprites.Count <= 0)
            {
                foreach (var sprite in transform.GetComponentsInChildren<SpriteRenderer>())
                {
                    sprites.Add(new Sprite(sprite, sprite.sortingLayerName));
                }
            }

            foreach (var sprite in sprites)
            {
                sprite.render.color = color;
            }

            if(!string.IsNullOrEmpty(sortingLayer))
                SetSortingLayer(sortingLayer);
        }
        public void ResetSpritesToDefault()
        {
            SetSpritesColor(Color.white);
            ResetSortingLayer();
        }
    }
}
