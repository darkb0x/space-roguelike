using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Player
{
    using Turret;

    public class PlayerPickObjects : MonoBehaviour
    {
        [Header("animator")]
        public Animator anim;
        [AnimatorParam("anim"), SerializeField] string anim_isPickSomethink;

        [Header("picked object")]
        [SerializeField, ReadOnly] GameObject pickedGameObject;
        [SerializeField] private Transform pickedGameObject_renderPosition; // where gameobject is be visible
        Transform pickedGameObject_transform;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PutCurrentGameobj();
            }

            if (pickedGameObject && pickedGameObject_transform)
            {
                pickedGameObject_transform.position = pickedGameObject_renderPosition.position;
            }
        }

        public void SetPickedGameobj(GameObject obj)
        {
            pickedGameObject = obj;
            pickedGameObject_transform = obj.transform;

            anim.SetBool(anim_isPickSomethink, true);
        }
        private void PutCurrentGameobj()
        {
            if (pickedGameObject)
            {
                if(pickedGameObject.TryGetComponent<TurretAI>(out TurretAI turret))
                {
                    turret.Put();
                }

                pickedGameObject_transform.position = transform.position;

                pickedGameObject = null;
                pickedGameObject_transform = null;

                anim.SetBool(anim_isPickSomethink, false);
            }
        }
    }
}
