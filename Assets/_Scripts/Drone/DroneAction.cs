using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DroneAction : ScriptableObject
{
    [HideInInspector] public PlayerController player;
    [HideInInspector] public DroneAI drone;

    #region drone action base
    public virtual void Init() { }
    public abstract void Run();
    public virtual void FixedRun() { }
    #endregion

    #region collision triggers
    public virtual void TriggerEnter(Collider2D col) { }
    public virtual void TriggerStay(Collider2D col) { }
    public virtual void TriggerExit(Collider2D col) { }
    #endregion
}
