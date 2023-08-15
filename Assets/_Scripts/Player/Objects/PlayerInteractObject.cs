using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

namespace Game.Player
{
    using Pick;
    using Input;

    public delegate void CollisionEnter(Collider2D coll);

    public class PlayerInteractObject : MonoBehaviour
    {
        private enum RenderTypeEnum { None, SingleObj, MulitplyObj }

        [Header("Action")]
        [SerializeField] private UnityEvent action;
        [Header("Interact rules")]
        [Tag, SerializeField] private string PlayerTag = "Player";
        [Space]

        [SerializeField, OnValueChanged("OnRenderTypeChanged")] private RenderTypeEnum RenderType = RenderTypeEnum.SingleObj;
        [SerializeField, ShowIf("RenderType", RenderTypeEnum.SingleObj)] private SpriteRenderer ObjRender;
        [SerializeField, ShowIf("RenderType", RenderTypeEnum.MulitplyObj)] private SpriteRenderer[] ObjRenderers;
        [SerializeField, HideIf("RenderType", RenderTypeEnum.None)] private Material OutlineMaterial;
        [SerializeField, ShowIf("RenderType", RenderTypeEnum.MulitplyObj)] private Material DefaultMaterial;
        [Space]

        [ReadOnly] public bool playerInZone = false;

        private PlayerInputHandler _input => InputManager.PlayerInputHandler;

        public CollisionEnter OnPlayerEnter;
        public CollisionEnter OnPlayerStay;
        public CollisionEnter OnPlayerExit;

        private PlayerPickObjects playerPick;
        private UIPanelManager UIPanelManager;

        private void OnRenderTypeChanged()
        {
            if(ObjRender != null)
                ObjRenderers = new SpriteRenderer[1] { ObjRender };

            ObjRender = null;
        }

        private void Start()
        {
            UIPanelManager = Singleton.Get<UIPanelManager>();

            if(ObjRender != null)
                DefaultMaterial = ObjRender.material;

            _input.InteractEvent.Performed += Interact;
        }
        private void OnDisable()
        {
            _input.InteractEvent.Performed -= Interact;
        }

        private void Interact()
        {
            if (!playerInZone)
                return;
            if (UIPanelManager.SomethinkIsOpened())
                return;
            if (playerPick.HaveObject)
                return;

            action.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(PlayerTag))
            {
                if (collision.TryGetComponent(out PlayerPickObjects pickObjects))
                    playerPick = pickObjects;

                OnPlayerEnter?.Invoke(collision);
                playerInZone = true;
            }
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag(PlayerTag))
            {
                OnPlayerStay?.Invoke(collision);
                playerInZone = true;

                EnableOutline(true);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(PlayerTag))
            {
                if (collision.TryGetComponent<PlayerPickObjects>(out PlayerPickObjects pickObjects))
                    playerPick = null;

                OnPlayerExit?.Invoke(collision);
                playerInZone = false;

                EnableOutline(false);
            }
        }  
        
        private void EnableOutline(bool enabled)
        {
            if (RenderType == RenderTypeEnum.None)
                return;

            if (DefaultMaterial == null)
            {
                Debug.LogWarning(gameObject.name + "/PlayerInteractObject.cs/EnableOutline(bool) | DefaultMaterial is null");
                return;
            }
            if(OutlineMaterial == null)
            {
                Debug.LogWarning(gameObject.name + "/PlayerInteractObject.cs/EnableOutline(bool) | OutlineMaterial is null");
                return;
            }

            if (RenderType == RenderTypeEnum.SingleObj)
            {
                ObjRender.material = enabled ? OutlineMaterial : DefaultMaterial;
            }
            else if(RenderType == RenderTypeEnum.MulitplyObj)
            {
                foreach (var obj in ObjRenderers)
                {
                    obj.material = enabled ? OutlineMaterial : DefaultMaterial;
                }
            }
        }
    }
}
