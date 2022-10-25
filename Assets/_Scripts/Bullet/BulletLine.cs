using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLine : MonoBehaviour
{
    [SerializeField] private Material[] animSprites;
    [SerializeField] private float time;

    public void Init(Vector3 target)
    {
        StartCoroutine(SpawnLine(GetComponent<LineRenderer>(), target));
    }

    IEnumerator SpawnLine(LineRenderer line, Vector3 point)
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, point);

        for (int i = 0; i < animSprites.Length; i++)
        {
            line.material = animSprites[i];
            yield return new WaitForSeconds(time);
        }

        Destroy(gameObject);
    }
}