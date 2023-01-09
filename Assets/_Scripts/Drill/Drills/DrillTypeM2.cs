using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Drill
{
    public class DrillTypeM2 : Drill
    {
        [SortingLayer, SerializeField] private string buildsSortingLayer;
        [Header("DrillTypeM2")]
        [SerializeField] private GameObject exploison;
        [SerializeField] private float expl_damage;
        [SerializeField] private LayerMask expl_layers;
        [SerializeField] private float expl_radius;
        [HorizontalLine(color: EColor.Red)]
        [SerializeField] private float expl_size;

        bool canBePicked = true;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, expl_radius);
        }

        public override void Initialize()
        {
            Pick();
        }

        public override void MiningEnded()
        {
            isMining = false;
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
            if (isMining)
                pick = false;

            return pick;
        }

        public void Pick()
        {
            backLegsSR.sortingLayerName = buildsSortingLayer;

            isPicked = true;
            mainColl.enabled = false;
            oreDetectColl.enabled = true;
            playerDetectColl.enabled = false;

            player.pickObjSystem.SetPickedGameobj(gameObject);
        }

        public override void Die()
        {
            Exploison obj = Instantiate(exploison, transform.position, Quaternion.Euler(0, 0, Random.Range(-180, 180))).GetComponent<Exploison>();
            obj.transform.localScale = new Vector3(expl_size, expl_size, expl_size);
            obj.Init(expl_damage, expl_layers, expl_radius);

            Destroy(gameObject);
        }
    }
}
