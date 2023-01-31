using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Player
{
    using Turret;
    using Drill;

    public class PlayerPickObjects : MonoBehaviour
    {
        [Header("animator")]
        public Animator anim;
        [AnimatorParam("anim"), SerializeField] string anim_isPickSomethink;

        [Header("picked object")]
        [SerializeField] private float pickRadius = 1.5f;
        [Space]
        [SerializeField] private Color pickedObjColor;
        [SerializeField, SortingLayer] private string blueprintSortingLayer;
        [Space]
        [ReadOnly] public GameObject pickedGameObject;
        [SerializeField] private Transform pickedGameObject_renderPosition; // where gameobject is be visible

        Transform pickedGameObject_transform;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, pickRadius);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (pickedGameObject)
                    PutCurrentGameobj();
                else
                    PickGameObj();
            }

            if (pickedGameObject && pickedGameObject_transform)
            {
                pickedGameObject_transform.position = pickedGameObject_renderPosition.position;
            }
        }

        public void SetPickedGameobj(GameObject obj)
        {
            if (pickedGameObject && pickedGameObject_transform)
                PutCurrentGameobj();

            pickedGameObject = obj;
            pickedGameObject_transform = obj.transform;

            anim.SetBool(anim_isPickSomethink, true);
        }
        public void PutCurrentGameobj(bool instatiadeObj = true)
        {
            if(instatiadeObj)
            {
                if (pickedGameObject.TryGetComponent<TurretAI>(out TurretAI turret))
                {
                    turret.Put();
                    pickedGameObject_transform.position = transform.position;
                }
                if (pickedGameObject.TryGetComponent<Drill>(out Drill drill))
                {
                    if (drill.CanPut())
                    {
                        drill.Put();
                    }
                    else
                    {
                        return;
                    }
                }
            }

            pickedGameObject = null;
            pickedGameObject_transform = null;

            anim.SetBool(anim_isPickSomethink, false);
        }
        private void PickGameObj()
        {
            Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, pickRadius);

            foreach (var coll in colls)
            {
                if (coll.TryGetComponent<DrillTypeM2>(out DrillTypeM2 drill))
                {
                    if(drill.CanPick())
                    {
                        drill.Pick();
                        break;
                    }
                }
            }
        }
    }
}
