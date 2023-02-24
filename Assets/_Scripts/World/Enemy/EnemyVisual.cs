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
        [SerializeField, AnimatorParam("Anim")] private string Anim_DieTrigger;
        [Space]
        [SerializeField] private float DisappearanceSpeed;
        [Space]
        [SerializeField] private GameObject AttackVisual;

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
            bool hit = Enemy.Attack();

            if(hit)
            {
                // Attack visual
                Animator animator = Instantiate(AttackVisual, Enemy.currentTarget.transform.position, Quaternion.identity).GetComponent<Animator>();
                animator.SetTrigger("Attack");
                Destroy(animator.gameObject, 0.5f);
            }
        }
        public void Death()
        {
            Anim.SetTrigger(Anim_DieTrigger);
        }
        private IEnumerator EnemyDisappearance()
        {
            while(Sprite.color.a > 0)
            {
                yield return null;
                float alphaValue = Mathf.MoveTowards(Sprite.color.a, 0, DisappearanceSpeed * Time.deltaTime);
                Sprite.color = new Color(1, 1, 1, alphaValue);
            }

            Destroy(Enemy.gameObject);
        }

        public void FlipSprite(bool right)
        {
            Sprite.flipX = right;
        }
    }
}
