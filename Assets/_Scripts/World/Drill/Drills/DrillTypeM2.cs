using System.Collections;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Drill
{
    public class DrillTypeM2 : Drill
    {
        [SortingLayer, SerializeField] private string PickSortingLayer;
        [Header("DrillTypeM2")]
        [SerializeField] private GameObject ExploisonEffect;

        private bool canBePicked = true;
        private bool inCooldown = false;

        public void Initialize()
        {
            base.Initialize(FindObjectOfType<Player.PlayerController>());

            BackLegsSR.sortingLayerName = PickSortingLayer;
        }
        public override void Initialize(Player.PlayerController p)
        {
            base.Initialize(p);

            BackLegsSR.sortingLayerName = PickSortingLayer;
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

        public override bool CanPick()
        {
            bool pick = true;

            if (inCooldown)
                return false;
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
            base.PlayerTakeItems();

            BackLegsSR.sortingLayerName = PickSortingLayer;

            isPicked = true;
            MainColl.enabled = false;
            OreDetectColl.enabled = true;
            PlayerDetectColl.enabled = false;

            player.Build.Pick(this);
        }

        public override void PlayerTakeItems()
        {
            if(ItemAmount > 0)
            {
                StartCoroutine(Cooldown());
            }

            base.PlayerTakeItems();
        }

        public override void Die()
        {
            ServiceLocator.GetService<Enemy.EnemySpawner>().RemoveTarget(EnemyTarget);
            player.Build.CleanPickedObject(this);

            Instantiate(ExploisonEffect, myTransform.position, Quaternion.identity);

            Destroy(gameObject);
        }

        private IEnumerator Cooldown()
        {
            float time = 0.02f;

            inCooldown = true;
            yield return new WaitForSeconds(time);
            inCooldown = false;
        }
    }
}
