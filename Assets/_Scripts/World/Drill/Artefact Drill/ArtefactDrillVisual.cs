using UnityEngine;
using NaughtyAttributes;
using TMPro;

namespace Game.Drill.SpecialDrill
{
    public class ArtefactDrillVisual : MonoBehaviour
    {
        [Header("Drill")]
        [SerializeField] private ArtefactDrill Drill;

        [Header("Animation")]
        [SerializeField] private Animator Anim;
        [SerializeField, AnimatorParam("Anim")] private string Anim_miningTrigger;

        [Header("Particles")]
        [SerializeField] private ParticleSystem MiningParticles;

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
