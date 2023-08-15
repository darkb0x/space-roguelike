using System.Collections;
using UnityEngine;

namespace Game.Turret.Bullets
{
    public class BulletTrail : MonoBehaviour
    {
        public void Init(Vector2 target)
        {
            StartCoroutine(SpawnTrail(GetComponent<TrailRenderer>(), target));
        }

        IEnumerator SpawnTrail(TrailRenderer trail, Vector2 point)
        {
            float time = 0;
            Transform trailTransform = trail.transform;
            Vector2 startPosition = trail.transform.position;

            while (time < 1)
            {
                trailTransform.position = Vector2.Lerp(trailTransform.position, point, time);
                time += Time.deltaTime / trail.time;

                yield return null;
            }
            trailTransform.position = point;

            Destroy(trail.gameObject, trail.time);
        }
    }
}
