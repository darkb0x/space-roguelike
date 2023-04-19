using UnityEngine;
using NaughtyAttributes;

namespace Game.Player.Pick
{
    using Turret;
    using Drill;
    using UnityEngine.InputSystem;
    using Visual;

    public class PlayerPickObjects : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private PlayerVisual Visual;

        [Header("picked object")]
        [SerializeField] private float pickRadius = 1.5f;
        [Space]
        [SerializeField] private Color pickedObjColor;
        [SerializeField, SortingLayer] private string blueprintSortingLayer;
        [Space]
        [ReadOnly] public GameObject pickedGameObject;
        [SerializeField] private Transform pickedGameObject_renderPosition; // where gameobject is be visible

        private Transform pickedGameObject_transform;

        public bool HaveObject { get; private set; }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, pickRadius);
        }

        private void Start()
        {
            GameInput.InputActions.Player.Build.performed += Pick;
        }

        private void Update()
        {
            if (pickedGameObject && pickedGameObject_transform)
            {
                pickedGameObject_transform.position = pickedGameObject_renderPosition.position;
            }
        }

        private void Pick(InputAction.CallbackContext obj)
        {
            if (pickedGameObject)
                PutCurrentGameobj();
            else
                PickGameObj();
        }

        public void SetPickedGameobj(GameObject obj)
        {
            if (pickedGameObject && pickedGameObject_transform)
                PutCurrentGameobj();

            pickedGameObject = obj;
            pickedGameObject_transform = obj.transform;

            Visual.PlayerPick(true);

            HaveObject = true;
        }
        public void PutCurrentGameobj(bool instatiadeObj = true)
        {
            if(instatiadeObj)
            {
                if (pickedGameObject.TryGetComponent<Turret>(out Turret turret))
                {
                    if(!turret.Put())
                    {
                        return;
                    }               
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

            Visual.PlayerPick(false);

            HaveObject = false;

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

        private void OnDisable()
        {
            GameInput.InputActions.Player.Build.performed -= Pick;
        }
    }
}
