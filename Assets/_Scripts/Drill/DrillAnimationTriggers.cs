using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Drill
{
    public class DrillAnimationTriggers : MonoBehaviour
    {
        [SerializeField] private Drill drillController;

        public void Enable_isMining()
        {
            drillController.isMining = true;
        }
        public void Disable_isMining()
        {
            drillController.isMining = false;
        }
    }
}
