using UnityEngine;

namespace Game.Turret
{
    using Bullets;

    public class TurretStandart : Turret
    {
        protected override void Attack()
        {
            float recoilRotation = Random.Range(-Recoil, Recoil);
            Bullet bullet = Instantiate(BulletPrefab, ShotPos.position, ShotPos.rotation).GetComponent<Bullet>();
            bullet.gameObject.transform.Rotate(0, 0, recoilRotation);
            bullet.Init(Damage);
        }
    }
}
