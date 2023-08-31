using UnityEngine;

namespace Game.Player.Components
{
    public class BuildController : PlayerComponent, IRequireUpdate, IInputDependant
    {
        public readonly float PickRadius;

        private readonly Color _pickedBuildColor;
        private readonly LayerMask _buildsLayer;
        private readonly string _blueprintSortingLayer;
        private readonly Transform _pickedBuildRenderPosition;

        public GameObject PickedBuildGameObject => _pickedBuildGameObject;
        public bool HaveBuild 
        {
            get => _pickedBuild != null && _pickedBuildGameObject != null;
        }

        private GameObject _pickedBuildGameObject;
        private ICraftableBuild _pickedBuild;

        public BuildController(ComponentConfig componentConfig, BuildConfig config) : base(componentConfig)
        {
            if (config.PickRadius <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(config.PickRadius));
            if (string.IsNullOrEmpty(config.BlueprintSortingLayer))
                throw new System.ArgumentOutOfRangeException(nameof(config.BlueprintSortingLayer));
            if (config.PickedBuildRenderPosition == null)
                throw new System.NullReferenceException(nameof(config.PickedBuildRenderPosition));

            PickRadius = config.PickRadius;
            _pickedBuildColor = config.PickedBuildColor;
            _buildsLayer = config.BuildsLayer;
            _blueprintSortingLayer = config.BlueprintSortingLayer;
            _pickedBuildRenderPosition = config.PickedBuildRenderPosition; 
        }

        public void AttachInput()
        {
            _input.BuildEvent += Build;
        }

        public void DetachInput()
        {
            _input.BuildEvent -= Build;
        }

        private void Build()
        {
            if (HaveBuild)
                Put();
            else
                Pick();
        }

        public void Update()
        {
            if (HaveBuild)
                _pickedBuildGameObject.transform.position = _pickedBuildRenderPosition.position;
        }

        public void Pick()
        {
            Collider2D[] colls = Physics2D.OverlapCircleAll(_player.transform.position, PickRadius, _buildsLayer);

            if (colls.Length == 0)
                return;

            foreach (var coll in colls)
            {
                if(coll.TryGetComponent(out ICraftableBuild build) && build.CanPick())
                {
                    Pick(build);
                }
            }
        }
        public void Pick(ICraftableBuild build)
        {
            if (HaveBuild)
                Put();

            _pickedBuild = build;
            _pickedBuildGameObject = build.GetGameObject();
            Debug.Log($"Pick {_pickedBuildGameObject.name}");

            _visual.PlayerPick(true);
        }
        public void Put()
        {
            if (!HaveBuild)
                return;

            if (_pickedBuild.CanPut())
            {
                _pickedBuild.Put();
                CleanPickedObject();
            }
        }

        public void CleanPickedObject()
        {
            _pickedBuildGameObject = null;
            _pickedBuild = null;

            _visual.PlayerPick(false);
        }
        public void CleanPickedObject(ICraftableBuild build)
        {
            if (_pickedBuild == build)
                CleanPickedObject();
        }
    }
}
