using CraftSystem.ScriptableObjects;
using UnityEngine;

namespace Game.CraftSystem.Craft
{
    [System.Serializable]
    public class TreeCraftContainer : CraftContainer
    {
        [SerializeField] private CSCraftContainerSO m_ContainerData;

        public CSCraftContainerSO ContainerData => m_ContainerData;
    }
}
