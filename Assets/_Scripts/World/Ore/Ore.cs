using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : MonoBehaviour
{
    public InventoryItem item;
    public int maxAmount;
    int amount;
    public bool canGiveOre = true;

    private void Start()
    {
        amount = maxAmount;
    }

    public void Give(int value)
    {
        amount = Mathf.Clamp(amount -= value, 0, maxAmount);

        if (amount <= 0)
            canGiveOre = false;
    }
}
