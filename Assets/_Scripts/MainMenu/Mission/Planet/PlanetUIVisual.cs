using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.Mission.Planet
{
    public class PlanetUIVisual : MonoBehaviour
    {
        private PlanetSO planetData;
        private Transform myTransform;
        private void Start()
        {
            myTransform = transform;
        }

        public void Initialize(PlanetSO so)
        {
            planetData = so;
        }

        public void Rotate(Transform point, float direction, float rangeFromPoint)
        {
            myTransform.position = (Vector2)point.position + new Vector2(Mathf.Sin(direction * Mathf.Deg2Rad), Mathf.Cos(direction * Mathf.Deg2Rad)) * rangeFromPoint;
        }
    }
}
