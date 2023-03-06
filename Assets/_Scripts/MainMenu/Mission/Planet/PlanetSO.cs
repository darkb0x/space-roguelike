using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.MainMenu.Mission.Planet
{
    [CreateAssetMenu(fileName = "Planet map", menuName = "Game/Mission/new Planet")]
    public class PlanetSO : ScriptableObject
    {
        public string MissionName;
        public Sprite MissionIcon;
        [Space]
        public Sprite PlanetSprite;
        [Scene] public int SceneId;
    }
}
