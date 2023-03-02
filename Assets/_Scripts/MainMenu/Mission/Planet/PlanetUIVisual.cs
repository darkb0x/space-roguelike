using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.Mission.Planet
{
    public class PlanetUIVisual : MonoBehaviour
    {
        public PlanetSO planetData { get; private set; }
        private Transform myTransform;

        public void Initialize(PlanetSO so)
        {
            myTransform = transform;

            planetData = so;
        }

        public void SelectPlanet()
        {

        }

        public void Rotate(Transform point, float direction, float rangeFromPoint)
        {
            myTransform.position = (Vector2)point.position + new Vector2(Mathf.Sin(direction * Mathf.Deg2Rad), Mathf.Cos(direction * Mathf.Deg2Rad)) * rangeFromPoint;
        }
    }
}
