using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Player.Pick
{
    public class PickedObjectPreRenderrer : MonoBehaviour
    {
        [SerializeField] private List<SpriteRenderer> SpritesToRender;
        [Space]
        public Color DefaultSpriteColor = Color.white;
        [SerializeField, SortingLayer] private string SortingLayer = "Ingnore Sort-Point";

        private List<SpriteRenderer> currentSprites = new List<SpriteRenderer>();

        private void Start()
        {
            foreach (var sprite in SpritesToRender)
            {
                GameObject spriteObj = new GameObject();
                spriteObj.name = "(Copy) " + sprite.name;

                spriteObj.transform.SetParent(transform);
                spriteObj.transform.position = sprite.transform.position;

                SpriteRenderer objRenderrer = spriteObj.AddComponent<SpriteRenderer>();
                objRenderrer.sprite = sprite.sprite;
                objRenderrer.sortingOrder = sprite.sortingOrder;
                objRenderrer.sortingLayerName = SortingLayer;

                currentSprites.Add(objRenderrer);
            }
            UpdateColor(DefaultSpriteColor);
        }

        public void UpdateColor(Color color)
        {
            foreach (var sprite in currentSprites)
            {
                sprite.color = color;
            }
        }
    }
}
