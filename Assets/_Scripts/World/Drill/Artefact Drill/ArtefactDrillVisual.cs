using UnityEngine;
using NaughtyAttributes;
using TMPro;

namespace Game.Drill.SpecialDrill
{
    public class ArtefactDrillVisual : MonoBehaviour
    {
        [Header("Drill")]
        [SerializeField] private ArtefactDrill Drill;

        [Header("UI Mining progress")]
        [SerializeField] private BossProgressBar MiningProgress;
        [Space]
        [SerializeField] private TextMeshProUGUI DrillIsMiningText;

        [Header("Animation")]
        [SerializeField] private Animator Anim;
        [SerializeField, AnimatorParam("Anim")] private string Anim_miningTrigger;

        [Header("Particles")]
        [SerializeField] private ParticleSystem MiningParticles;

        private void Start()
        {
            EnableMiningProgressVisual(false);
        }

        public void EnableMiningProgressVisual(bool enabled)
        {
            MiningProgress.EnableProgressBar(enabled);
        }

        public void UpdateMiningProgress(float current, float max, string titleText = "Drill is mining!")
        {
            MiningProgress.UpdateProgressBar(current, max);

            DrillIsMiningText.text = titleText;
        }

        public void MiningAnimation()
        {
            Anim.SetTrigger(Anim_miningTrigger);
        }
        
        private void Anim_Mining()
        {
            MiningParticles.Play();
            Drill.Mining();
        }
    }
}
