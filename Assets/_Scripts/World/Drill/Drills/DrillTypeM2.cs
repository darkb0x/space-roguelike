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

        public bool CanPick()
        {
            bool pick = true;

            if (!canBePicked)
                pick = false;
            if (isPicked)
                pick = false;
            if (IsMining)
                pick = false;
            if (inCooldown)
                return false;

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

            player.pickObjSystem.SetPickedGameobj(gameObject);
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
            Singleton.Get<Enemy.EnemySpawner>().RemoveTarget(EnemyTarget);
            player.pickObjSystem.SetPickedGameobj(null);

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
