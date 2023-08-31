using UnityEngine;
using NaughtyAttributes;

namespace Game.Player.Components
{
    [System.Serializable]
    public struct BuildConfig
    {
        public float PickRadius;
        public LayerMask BuildsLayer;
        [Space]
        public Color PickedBuildColor;
        [SortingLayer] public string BlueprintSortingLayer;
        [Space]
        public Transform PickedBuildRenderPosition;
    }
}
