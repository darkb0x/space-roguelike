using UnityEngine;
using NaughtyAttributes;

namespace Game.Drill
{
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

            Singleton.Get<Enemy.EnemySpawner>().RemoveTarget(EnemyTarget);
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
