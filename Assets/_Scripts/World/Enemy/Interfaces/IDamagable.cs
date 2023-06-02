namespace Game
{
    public interface IDamagable
    {
        public void Damage(float dmg, Enemy.EnemyTarget enemyTarget)
        {
            if (enemyTarget.IsDamaging)
            {
                enemyTarget.Health -= dmg;

                if (enemyTarget.Health <= 0)
                {
                    Die();
                }
            }
        }
        public void Die();
    }
}
