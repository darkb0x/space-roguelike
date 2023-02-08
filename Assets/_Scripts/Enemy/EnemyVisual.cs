using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.Enemy
{
    public class EnemyVisual : MonoBehaviour
    {
        [SerializeField] private EnemyAI Enemy;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer Sprite;
        [SerializeField] private Animator Anim;
        [SerializeField, AnimatorParam("Anim")] private string Anim_IsRunningBool;
        [SerializeField, AnimatorParam("Anim")] private string Anim_AttackTrigger;
        [Space]
        [SerializeField] private Transform AttackVisual;
        [SerializeField] private Animator AttackVisualAnim;

        private void Update()
        {
            if(Enemy == null)
            {
                Debug.LogWarning($"{gameObject.name}/EnemyVisual.cs/Enemy is null!");
            }

            // Running
            Anim.SetBool(Anim_IsRunningBool, !Enemy.reachedEndOfPath);
        }

        public void StartAttacking()
        {
            // Body animation
            Anim.SetTrigger(Anim_AttackTrigger);
        }
        public void Attack()
        {
            Enemy.Attack();

            // Attack visual
            Vector2 difference = transform.position - Enemy.currentTarget.transform.position;
            float rotation = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            float offset = 180f;
            AttackVisual.rotation = Quaternion.Euler(0, 0, rotation + offset);
            AttackVisualAnim.SetTrigger("Attack");
        }

        public void FlipSprite(bool right)
        {
            Sprite.flipX = right;
        }
    }
}
