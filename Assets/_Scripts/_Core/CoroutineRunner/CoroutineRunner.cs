using System.Collections;
using UnityEngine;

namespace Game
{
    public class CoroutineRunner : MonoBehaviour, IService, ICoroutineRunner
    {
        public Coroutine RunCoroutine(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);   
        }
        public void CancelCoroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
        public void CancelAllCoroutines()
        {
            StopAllCoroutines();
        }
    }
}