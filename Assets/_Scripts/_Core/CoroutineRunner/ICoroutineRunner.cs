using System.Collections;
using UnityEngine;

namespace Game
{
    public interface ICoroutineRunner
    {
        public Coroutine RunCoroutine(IEnumerator coroutine);
        public void CancelCoroutine(Coroutine coroutine);
        public void CancelAllCoroutines();
    }
}
