using UnityEngine;

namespace Game.Drill.SpecialDrill
{
    using Game.Inventory;

    public class ArtefactDrill : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private ArtefactDrillVisual Visual;

        [Header("Artefact")]
        [SerializeField] private float m_ArtefactHealth;
        [SerializeField] private ItemData Artefact;

        [Header("Mining")]
        [SerializeField] private float Damage;
        [SerializeField] private float m_TimeBtwAttacks;

        private float artefactHealth;
        private float timeBtwAttacks;
        private bool isMining = false;

        private void Start()
        {
            artefactHealth = m_ArtefactHealth;
            timeBtwAttacks = m_TimeBtwAttacks;
        }

        private void Update()
        {
            if(isMining)
            {
                if(timeBtwAttacks <= 0)
                {
                    Visual.MiningAnimation();
                    timeBtwAttacks = m_TimeBtwAttacks;
                }
                else
                {
                    timeBtwAttacks -= Time.deltaTime;
                }
            }
        }

        public void Mining()
        {
            if (artefactHealth > 0)
            {
                artefactHealth -= Damage;
                Visual.UpdateMiningProgress(artefactHealth, m_ArtefactHealth);
            }
            else
            {
                EndMining();
            }
        }

        public void StartMining()
        {
            Visual.EnableMiningProgressVisual(true);
            Visual.UpdateMiningProgress(artefactHealth, m_ArtefactHealth);
            isMining = true;
        }

        public void EndMining()
        {
            ServiceLocator.GetService<IInventory>().AddItem(Artefact);
            Visual.UpdateMiningProgress(artefactHealth, m_ArtefactHealth);
            Visual.EnableMiningProgressVisual(false);
            isMining = false;
        }
    }
}
