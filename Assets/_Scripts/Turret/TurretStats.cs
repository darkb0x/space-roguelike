using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "turret stats", menuName = "Turret/Stats/new turret stats")]
public class TurretStats : ScriptableObject
{
    [System.Serializable]
    public struct Item
    {
        public InventoryItem item;
        public int amount;
    }

    public GameObject bulletPrefab;
    public int damage;
    public float timeBtwAttack = 0.3f;

    [Space]

    public List<Item> DroppedItems;
}
