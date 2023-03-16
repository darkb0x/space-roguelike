using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Drill.SpecialDrill
{
    public class ArtefactDrillVisual : MonoBehaviour
    {
        [Header("Mining progress")]
        [SerializeField] private GameObject MiningProgressVisual;
        [Space]
        [SerializeField] private Image MiningProgressImage;
        [SerializeField] private TextMeshProUGUI MiningProgressText;
        [SerializeField] private TextMeshProUGUI DrillIsMiningText;

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
            MiningProgressText.text = percent + "%";
            DrillIsMiningText.text = titleText;
        }
    }
}
