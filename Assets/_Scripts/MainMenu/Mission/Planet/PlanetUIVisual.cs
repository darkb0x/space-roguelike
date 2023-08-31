using UnityEngine;
using UnityEngine.UI;

namespace Game.MainMenu.MissionChoose.Planet.Visual
{

    public class PlanetUIVisual : MonoBehaviour
    {
        [SerializeField] private Image PlanetImage;

        public PlanetSO planetData { get; private set; }
        private Transform myTransform;

        private MissionChooseManager MissionChooseManager;

        public void Initialize(PlanetSO so)
        {
            MissionChooseManager = ServiceLocator.GetService<MissionChooseManager>();

            myTransform = transform;

            planetData = so;

            PlanetImage.sprite = planetData.PlanetSprite;
        }

        public void SelectPlanet()
        {
            MissionChooseManager.SelectMission(planetData); 
        }

        public void Rotate(Transform point, float direction, float rangeFromPoint)
        {
            myTransform.position = (Vector2)point.position + new Vector2(Mathf.Sin(direction * Mathf.Deg2Rad), Mathf.Cos(direction * Mathf.Deg2Rad)) * rangeFromPoint;
        }
    }
}
