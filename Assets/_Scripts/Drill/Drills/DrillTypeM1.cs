using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Drill
{
    /*
     * What the M1 type?
     * The M1 is mining faster than M2 version but, it mines a 50% of all ore and saves extracted resources.
     * Cant be moved after extracting.
    */

    using Player.Inventory;

    public class DrillTypeM1 : Drill
    {
        [Header("DrillTypeM1")]
        [SerializeField, Range(0, 100)] private int maxExtractedPercentFromOre = 50;
        private int amountOreFromPercent = 0;
        [Space]
        [SerializeField] private ParticleSystem brokeDrillParticle;
        [SerializeField] private ParticleSystem smokePatricle;
        [Space]
        [SerializeField] private SpriteRenderer backLegsSR;
        [NaughtyAttributes.SortingLayer, SerializeField] private string worldSortingLayer; 

        public override void Mine()
        {
            if(currentOre.amount <= amountOreFromPercent)
            {
                MiningEnded();
                return;
            }

            base.Mine();
        }

        public override void MiningEnded()
        {
            PlayerInventory.playerInventory.AddItem(item, amount);

            currentOre.canGiveOre = false;
            isMining = false;

            anim.SetTrigger("Die");

            brokeDrillParticle.Play();
            smokePatricle.Play();
        }

        public override void Put()
        {
            amountOreFromPercent = currentOre.maxAmount / 100 * maxExtractedPercentFromOre;

            backLegsSR.sortingLayerName = worldSortingLayer;

            base.Put();
        }
    }
}
