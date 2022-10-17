using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour
{
    public test1[] rotators;
    public float rotationPower;
    float direction = 0;

    void Update()
    {
        direction += Time.deltaTime * rotationPower;

        int i = 0;
        foreach (var rotator in rotators)
        {
            rotator.RotationUpdate(transform, (i / (float)(rotators.Length)) * 360 + direction);
            i++;
        } 
    }
}
