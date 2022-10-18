using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private int maxHp;
    public int hp;
    [Space]
    [SerializeField] private int protection;
    [SerializeField] private int hp_protection;
    [Space]
    [SerializeField] private GameObject hpBar_object;
    [SerializeField] private Image hpBar_image;

    private void Start()
    {
        hp = maxHp;
        hpBar_object.SetActive(false);
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

        if(hp < maxHp)
        {
            hpBar_object.SetActive(true);
            hpBar_image.fillAmount = (float)hp / (float)maxHp;
        }

        if (hp <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
