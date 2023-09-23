using UnityEngine;
using System;

namespace Game.Session.Artefact
{
    using Game.Inventory;

    public class ArtefactDrill : MonoBehaviour, IEntryComponent
    {
        private const float WAIT_TIME_AFTER_REPAIR = 0.6f;

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

        [NaughtyAttributes.Button]
        public void StartMining()
        {
            OnMiningStarted?.Invoke();
            _isMining = true;
            _timeBtwAttacks = WAIT_TIME_AFTER_REPAIR;
        }

        public void EndMining()
        {
            ServiceLocator.GetService<IInventory>().AddItem(Artefact);
            OnMiningEnded?.Invoke();
            _isMining = false;
        }
    }
}
