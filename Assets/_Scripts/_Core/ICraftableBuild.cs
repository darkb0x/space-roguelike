namespace Game
{
    using Player;
    using UnityEngine;

    public interface ICraftableBuild
    {
        public void Initialize(PlayerController p);
        public GameObject GetGameObject();
        public bool CanPut();
        public void Put();
        public bool CanPick();
    }
}
