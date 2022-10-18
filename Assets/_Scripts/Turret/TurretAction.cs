using System.Collections.Generic;
using UnityEngine;

public abstract class TurretAction : ScriptableObject
{
    [System.Serializable]
    public struct Item
    {
        public InventoryItem item;
        public int amount;
    }

    public List<Item> DroppedItems;

    [HideInInspector] public PlayerController player;
    [HideInInspector] public TurretAI turret;

    #region drone action base
    public virtual void Init() { }
    public abstract void Run();
    public virtual void FixedRun() { }
    #endregion

    #region collision triggers
    public virtual void TriggerEnter(Collider2D col, TurretAI _turret) { }
    public virtual void TriggerStay(Collider2D col, TurretAI _turret) { }
    public virtual void TriggerExit(Collider2D col, TurretAI _turret) { }
    #endregion
}
