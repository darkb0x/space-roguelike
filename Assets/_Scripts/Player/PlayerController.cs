using System.Collections.Generic;
using UnityEngine;

namespace Game.Player
{
    using Input;
    using Components;
    using Game.Player.State;
    using System;

    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, IDamagable, IMovableTarget
    {
        [Header("Oxygen")]
        [SerializeField] private OxygenConfig OxygenConfig;

        [Header("Health")]
        [SerializeField] private HealthConfig HealthConfig;

        [Header("Movement")]
        [SerializeField] private MovementConfig MovementConfig;

        [Header("Build")]
        [SerializeField] private BuildConfig BuildConfig;

        [Header("Visual")]
        [SerializeField] private PlayerVisual Visual;

        [Header("Components")]
        public Collider2D MainCollider;
        [SerializeField] private Enemy.EnemyTarget EnemyTarget;

        // Components
        public Action<PlayerComponent> OnComponentChangedEnabled;
        public HealthController Health;
        public OxygenController Oxygen;
        public MovementController Movement;
        public BuildController Build;

        private List<IRequireUpdate> _updatableComponents;
        private List<IRequireFixedUpdate> _fixedUpdatableComponents;

        // State
        public PlayerState CurrentState { get { return _state; } }
        private PlayerState _state;
        public PlayerState DefaultState { get; } = new PlayerDefaultState();
        public PlayerState DeadState { get; } = new PlayerDeadState();
        public PlayerState InvulnerabityState { get; } = new PlayerInvulnerabilityState();
        public PlayerState StandingState { get; } = new PlayerStandingState();

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, BuildConfig.PickRadius);
        }

        private void Awake()
        {
            InitializeComponents();
            InitializeStates();
        }

        private void InitializeComponents()
        {
            ComponentConfig componentConfig = new ComponentConfig()
            {
                Player = this,
                Visual = Visual,
                Input = InputManager.PlayerInputHandler
            };

            Oxygen = new OxygenController(componentConfig, OxygenConfig);
            Health = new HealthController(componentConfig, HealthConfig);
            Movement = new MovementController(componentConfig, MovementConfig);
            Build = new BuildController(componentConfig, BuildConfig);

            _updatableComponents = new List<IRequireUpdate>()
            {
                Oxygen,
                Movement,
                Build
            };
            _fixedUpdatableComponents = new List<IRequireFixedUpdate>()
            {
                Movement
            };

            Oxygen.OnEnableChanged += () => OnComponentChangedEnabled?.Invoke(Oxygen);
            Health.OnEnableChanged += () => OnComponentChangedEnabled?.Invoke(Health);
            Movement.OnEnableChanged += () => OnComponentChangedEnabled?.Invoke(Movement);

            Build.AttachInput();
        }
        private void InitializeStates()
        {
            DefaultState.Initialize(this);
            DeadState.Initialize(this);
            InvulnerabityState.Initialize(this);
            StandingState.Initialize(this);

            SetState(DefaultState);
        }

        private void Start()
        {
            EnemyTarget.Initialize(this, this);
        }

        private void OnDisable()
        {
            Build.DetachInput();

            Oxygen.OnEnableChanged -= () => OnComponentChangedEnabled?.Invoke(Oxygen);
            Health.OnEnableChanged -= () => OnComponentChangedEnabled?.Invoke(Health);
            Movement.OnEnableChanged -= () => OnComponentChangedEnabled?.Invoke(Movement);
        }

        private void Update()
        {
            foreach (var component in _updatableComponents)
            {
                component.Update();
            }
        }

        private void FixedUpdate()
        {
            foreach (var component in _fixedUpdatableComponents)
            {
                component.FixedUpdate();
            }
        }

        public void SetState(PlayerState state)
        {
            _state = state;
            _state.Enable();
        }
        public void SetComponentEnabled(PlayerComponent component, bool enabled)
        {
            if (enabled)
                component.Enable();
            else
                component.Disable();
        }
        private void SetComponentEnabled(bool enabled)
        {
            SetComponentEnabled(Health, enabled);
            SetComponentEnabled(Oxygen, enabled);
            SetComponentEnabled(Movement, enabled);
            SetComponentEnabled(Build, enabled);
        }
        public void EnableAllComponents()
        {
            SetComponentEnabled(true);
        }
        public void DisableAllComponents()
        {
            SetComponentEnabled(false);
        }

        #region IMovableTarget
        public Vector3 GetMoveDirection()
        {
            if(_state == DefaultState)
            {
                return Movement.GetMoveDirection();
            } 
            else
            {
                return Vector3.zero;
            }
        }
        #endregion

        #region IDamagable
        void IDamagable.Damage(float dmg, Enemy.EnemyTarget enemyTarget)
        {
            Health.TakeDamage(dmg);
        }
        void IDamagable.Die()
        {
            Singleton.Get<Enemy.EnemySpawner>().RemoveTarget(EnemyTarget);
            Health.Die();
        }
        #endregion

        #region Animation Control
        public void StopPlayerMove(Transform posTransform)
        {
            StopPlayerMove();
            transform.position = posTransform.position;
        }
        public void StopPlayerMove()
        {
            SetState(StandingState);
        }
        public void ContinuePlayerMove()
        {
            SetState(DefaultState);
        }

        public void LockPlayerPosition(Transform posPosition)
        {
            SetState(StandingState);
            transform.SetParent(posPosition);
            transform.localPosition = Vector2.zero;
            MainCollider.enabled = false;
            SetComponentEnabled(Health, false);
        }
        public void UnlockPlayerPosition()
        {
            transform.SetParent(null);
            SetState(DefaultState);
            MainCollider.enabled = true;
            SetComponentEnabled(Health, true);
        }
        #endregion
    }
}