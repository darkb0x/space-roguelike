using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Drill
{
    public class DrillTypeM2 : Drill
    {
        [SortingLayer, SerializeField] private string PickSortingLayer;
        [Header("DrillTypeM2")]
        [SerializeField] private GameObject ExploisonEffect;

        bool canBePicked = true;

        public override void Initialize()
        {
            Pick();
        }

        public override void MiningEnded()
        {
            IsMining = false;
            canBePicked = true;
        }

        public override void Put()
        {
            base.Put();

            canBePicked = false;
        }

        public bool CanPick()
        {
            bool pick = true;

            if (!canBePicked)
                pick = false;
            if (isPicked)
                pick = false;
            if (IsMining)
                pick = false;

            return pick;
        }

        public void Pick()
        {
            PlayerTakeItems();

            BackLegsSR.sortingLayerName = PickSortingLayer;

            isPicked = true;
            MainColl.enabled = false;
            OreDetectColl.enabled = true;
            PlayerDetectColl.enabled = false;

            player.pickObjSystem.SetPickedGameobj(gameObject);
        }

        public override void Die()
        {
            Instantiate(ExploisonEffect, myTransform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
