using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float maxHp;
    public float hp;
    [Space]
    [SerializeField] private float protection;
    [SerializeField] private float hp_protection;
    [Space]
    [SerializeField] private GameObject hpBar_object;
    [SerializeField] private Image hpBar_image;

    private void Start()
    {
        hp = maxHp;
        hpBar_object.SetActive(false);
    }

    public void TakeDamage(float value)
    {
        float dmg = value - protection;

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
            hp = Mathf.Clamp(hp - dmg, 0, maxHp);
        }

        if(hp < maxHp)
        {
            hpBar_object.SetActive(true);
            hpBar_image.fillAmount = hp / maxHp;
        }

        if (hp <= 0)
            Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
