using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CraftSystem
{
    using Player;
    using Drill;
    using Turret;
    using CraftSystem.Editor.ScriptableObjects;

    [RequireComponent(typeof(PlayerInteractObject))]
    [RequireComponent(typeof(Enemy.EnemyTarget))]
    public class Workbanch : MonoBehaviour, IDamagable
    {
        CSManager craftSystem;
        PlayerController player;
        Transform myTransform;

        private void Start()
        {
            craftSystem = FindObjectOfType<CSManager>();
            player = FindObjectOfType<PlayerController>();
            myTransform = transform;

            GetComponent<Enemy.EnemyTarget>().Initialize(this);
        }

        public void Craft(Craft obj)
        {
            GameObject craftedObj = Instantiate(obj._prefab, myTransform.position, Quaternion.identity);

            if(obj is CraftTurret craftData_turret)
            {
                Turret turret = craftedObj.GetComponent<Turret>();
                turret.Initialize(player, craftData_turret._turretAI, craftData_turret._turretData);
            }

            player.ContinuePlayerMove();
        }

        public void OpenCraftMenu()
        {
            if (!player.pickObjSystem.pickedGameObject)
            {
                player.StopPlayerMove(transform.position);
                craftSystem.OpenMenu(this);
            }
        }

        public void Damage(float dmg)
        {
            Debug.Log(gameObject.name + " damaged!");
        }

        public void Die()
        {
            Destroy(gameObject);
        }
    }
}
