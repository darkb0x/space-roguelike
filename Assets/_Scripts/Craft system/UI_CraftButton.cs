using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftButton : MonoBehaviour
{
    [Header("item")]
    [SerializeField] private CraftItem item;
    public bool isLearned = false;

    [Header("ui")]
    [SerializeField] private Image icon;
    [SerializeField] private Text itemName;
    [Space]
    [SerializeField] private Transform resourceIcons;
    [SerializeField] private GameObject resourceUIPrefab;
}
