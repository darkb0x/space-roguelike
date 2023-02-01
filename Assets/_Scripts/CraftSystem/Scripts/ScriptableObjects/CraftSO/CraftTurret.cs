using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem.Editor.ScriptableObjects
{
    using Turret;
    using Turret.AI;

    [CreateAssetMenu(fileName = "Node turret craft", menuName = "CraftSystem/new node turret craft")]
    public class CraftTurret : Craft
    {
        [SerializeField] private TurretAI TurretAI;
        [SerializeField] private TurretData TurretData;

        public TurretAI _turretAI => TurretAI;
        public TurretData _turretData => TurretData;
    }
}
