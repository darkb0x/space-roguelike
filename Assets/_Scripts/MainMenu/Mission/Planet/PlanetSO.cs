using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.MainMenu.Mission.Planet
{
    [CreateAssetMenu(fileName = "Planet map", menuName = "Game/Mission/new Planet")]
    public class PlanetSO : ScriptableObject
    {
        [Scene] public int SceneId;
    }
}
