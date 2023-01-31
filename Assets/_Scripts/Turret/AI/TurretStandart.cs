using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Turret.AI
{
    [CreateAssetMenu(fileName = "Turret standart AI", menuName = "Turret/AI/new standart AI")]
    public class TurretStandart : TurretAI
    {
        public override void Run()
        {
            if(!turret.isPicked)
                base.Run();
        }
    }
}
