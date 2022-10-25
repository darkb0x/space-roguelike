using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CraftSystem : MonoBehaviour
{
    // singletone
    public static CraftSystem craftSystem;
    private void Awake() => craftSystem = this;

    #region struct objects
    [System.Serializable]
    public struct techLine
    {
        public string name;
        public UI_CraftButton[] items;
    }
    #endregion

    [Header("tech tree")]
    [SerializeField] private List<techLine> techTree = new List<techLine>();

    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
}
