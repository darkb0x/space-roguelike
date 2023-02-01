using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Editor.ScriptableObjects
{
    public class Craft : ScriptableObject
    {
        [SerializeField] private GameObject Prefab;

        public GameObject _prefab => Prefab;
    }
}
