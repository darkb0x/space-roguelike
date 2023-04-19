using UnityEngine;

namespace Game.Drill
{
    public class DrillAnimationTriggers : MonoBehaviour
    {
        [SerializeField] private Drill drillController;

        public void Enable_isMining()
        {
            drillController.IsMining = true;
        }
        public void Disable_isMining()
        {
            drillController.IsMining = false;
        }
    }
}
