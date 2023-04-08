using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

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
        [SerializeField] private Player.BreakingBuildObject BreakingBuildObject;
        [Space]
        [SerializeField, Range(0, 100)] private int MaxExtractedPercentFromOre = 50;
        [SerializeField, ReadOnly] private int AmountOreFromPercent = 0;
        [Space]
        [SerializeField, AnimatorParam("Anim")] private string anim_DieTrigger = "Die";
        [Space]
        [SerializeField] private GameObject exploisonGameObj;
        [SerializeField] private ParticleSystem smokePatricle;


        public override void Mine()
        {
            if(allExtractedOre >= AmountOreFromPercent)
            {
                MiningEnded();
                return;
            }

            base.Mine();
        }

        public override void MiningEnded()
        {
            IsMining = false;

            Anim.SetTrigger(anim_DieTrigger);

            Enemy.EnemySpawner.Instance.RemoveTarget(EnemyTarget);
            exploisonGameObj.SetActive(true);
            BreakingBuildObject.DisableBreaking();

            smokePatricle.Play();
        }

        public override void Put()
        {
            base.Put();

            AmountOreFromPercent = Mathf.RoundToInt((float)CurrentOre.MaxAmount / 100 * MaxExtractedPercentFromOre);
        }
    }
}
