using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private int maxHp;
    [SerializeField] private int hp;
    [Space]
    [SerializeField] private int protection;
    [SerializeField] private int hp_protection;

    private void Start()
    {
        hp = maxHp;
    }

    public void TakeDamage(int value)
    {
        Debug.Log("Enemy got damage.");
        int dmg = value - protection;

        if(dmg <= 0)
        {
            hp_protection--;
            if(hp_protection <= 0)
            {
                protection = 0;
            }
        }
        else
        {
            hp -= dmg;
        }

        if (hp <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
