using UnityEngine;
using System;

namespace Game.Drill.SpecialDrill
{
    using Game.Inventory;

    public class ArtefactDrill : MonoBehaviour, IEntryComponent
    {
        [Header("Visual")]
        [SerializeField] private ArtefactDrillVisual Visual;

        [Header("Artefact")]
        [SerializeField] private float m_ArtefactHealth;
        [SerializeField] private ItemData Artefact;

        [Header("Mining")]
        [SerializeField] private float Damage;
        [SerializeField] private float m_TimeBtwAttacks;

        public Action OnMiningStarted;
        public Action OnMiningEnded;
        public Action<float, float> OnMiningUpdate;

        private float _artefactHealth;
        private float _timeBtwAttacks;
        private bool _isMining = false;

        public void Initialize()
        {
            _artefactHealth = m_ArtefactHealth;
            _timeBtwAttacks = m_TimeBtwAttacks;
        }

        private void Update()
        {
            if(_isMining)
            {
                if(_timeBtwAttacks <= 0)
                {
                    Visual.MiningAnimation();
                    _timeBtwAttacks = m_TimeBtwAttacks;
                }
                else
                {
                    _timeBtwAttacks -= Time.deltaTime;
                }
            }
        }

        public void Mining()
        {
            if (_artefactHealth > 0)
            {
                _artefactHealth -= Damage;
                OnMiningUpdate?.Invoke(_artefactHealth, m_ArtefactHealth);
            }
            else
            {
                EndMining();
            }
        }

        public void StartMining()
        {
            OnMiningStarted?.Invoke();
            _isMining = true;
        }

        public void EndMining()
        {
            ServiceLocator.GetService<IInventory>().AddItem(Artefact);
            OnMiningEnded?.Invoke();
            _isMining = false;
        }
    }
}
