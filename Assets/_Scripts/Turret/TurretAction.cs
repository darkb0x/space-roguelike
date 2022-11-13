using System.Collections.Generic;
using UnityEngine;

namespace Game.Turret
{
    using Player;

    public abstract class TurretAction : ScriptableObject
    {
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
}
