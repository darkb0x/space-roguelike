using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Enemy
{
    [CreateAssetMenu(fileName = "Enemy data", menuName = "Game/new Enemy data")]
    public class EnemyData : ScriptableObject
    {
        [field: SerializeField, NaughtyAttributes.ShowAssetPreview] public GameObject EnemyPrefab { get; private set; }
        [field: SerializeField] public Sprite EnemyIcon;
        [field: Space]
        [field: SerializeField] public float Health { get; private set; }
        [field: SerializeField] public float Protection { get; private set; }
        [field: Space]
        [field: SerializeField] public float Damage { get; private set; }
        [field:Space]
        [field: SerializeField] public int Cost { get; set; }
    }
}
