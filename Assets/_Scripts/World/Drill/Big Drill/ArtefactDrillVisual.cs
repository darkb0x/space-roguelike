using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;

namespace Game.Drill.SpecialDrill
{
    public class ArtefactDrillVisual : MonoBehaviour
    {
        [Header("Drill")]
        [SerializeField] private ArtefactDrill Drill;

        [Header("UI Mining progress")]
        [SerializeField] private GameObject MiningProgressVisual;
        [Space]
        [SerializeField] private Image MiningProgressImage;
        [SerializeField] private Image MiningDamageProgressImage;
        [SerializeField] private TextMeshProUGUI MiningProgressText;
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
            MiningProgressVisual.SetActive(enabled);
        }

        public void UpdateMiningProgress(float current, float max, int percent, string titleText = "Drill is mining!")
        {
            MiningProgressImage.fillAmount = current / max;

            StartCoroutine(UpdateDamageProgress(current, max));

            MiningProgressText.text = percent + "%";
            DrillIsMiningText.text = titleText;
        }

        private IEnumerator UpdateDamageProgress(float current, float max)
        {
            float waitTime = 0.2f;
            float speed = 0.2f;

            yield return new WaitForSeconds(waitTime);

            while(MiningDamageProgressImage.fillAmount != MiningProgressImage.fillAmount)
            {
                MiningDamageProgressImage.fillAmount = Mathf.MoveTowards(MiningDamageProgressImage.fillAmount, MiningProgressImage.fillAmount, speed * Time.deltaTime);

                yield return null;
            }

            MiningDamageProgressImage.fillAmount = MiningProgressImage.fillAmount;
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
