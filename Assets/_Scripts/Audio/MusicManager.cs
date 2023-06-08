using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance;

        public AudioSource AudioSource;
        [Space]
        [SerializeField] private float TimeTransitionBtwMusic = 1.2f;
        [SerializeField, Range(0, 1)] private float MaxVolume = 0.7f;

        private AudioClip currentMusic;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetMusic(AudioClip clip, bool loop)
        {
            AudioSource.loop = loop;

            if(clip != null)
            {
                StartCoroutine(MusicTransition(clip));
            }
            else
            {
                StartCoroutine(MusicTransition());
            }
        }

        private IEnumerator MusicTransition(AudioClip next)
        {
            if(currentMusic != null)
            {
                while (AudioSource.volume > 0)
                {
                    AudioSource.volume = Mathf.MoveTowards(AudioSource.volume, 0, TimeTransitionBtwMusic * Time.deltaTime);
                    yield return null;
                }
            }
            else
            {
                AudioSource.volume = 0;
            }

            currentMusic = next;

            AudioSource.clip = currentMusic;
            AudioSource.Play();
            while (AudioSource.volume < MaxVolume)
            {
                AudioSource.volume = Mathf.MoveTowards(AudioSource.volume, MaxVolume, TimeTransitionBtwMusic * Time.deltaTime);
                yield return null;
            }
        }
        private IEnumerator MusicTransition()
        {
            while (AudioSource.volume > 0)
            {
                AudioSource.volume = Mathf.MoveTowards(AudioSource.volume, 0, TimeTransitionBtwMusic * Time.deltaTime);
                yield return null;
            }
        }

        private void OnEnable()
        {
            AudioSource.UnPause();
        }
    }
}
