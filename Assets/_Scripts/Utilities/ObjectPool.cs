using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utilities
{
    public class ObjectPool<TPoolObject>
    {
        private readonly Func<TPoolObject> _preloadFunc;
        private readonly Action<TPoolObject> _getAction;
        private readonly Action<TPoolObject> _returnAction;

        private Queue<TPoolObject> _pool = new Queue<TPoolObject>();
        private List<TPoolObject> _active = new List<TPoolObject>();

        public ObjectPool(Func<TPoolObject> preloadFunc, Action<TPoolObject> getAction, Action<TPoolObject> returnAction, int preloadCount)
        {
            _preloadFunc = preloadFunc;
            _getAction = getAction;
            _returnAction = returnAction;

            if(preloadFunc == null)
            {
                Debug.LogError("ObjectPool.cs | constructor ObjectPool(Func, Action, Action, int) | preloadFunc(Func) is null!");
                return;
            }

            for (int i = 0; i < preloadCount; i++)
            {
                Return(preloadFunc());
            }
        }

        public TPoolObject Get(bool takeFirstIfNoFreeObjects = false)
        {
            TPoolObject item;
            if (takeFirstIfNoFreeObjects)
            {
                if(_pool.Count == 0)
                {
                    Return(_active[0]);
                }
                item = _pool.Dequeue();
            }
            else
            {
                item = _pool.Count > 0 ? _pool.Dequeue() : _preloadFunc();
            }
            _getAction(item);
            _active.Add(item);

            return item;
        }

        public void Return(TPoolObject item)
        {
            _returnAction(item);
            _pool.Enqueue(item);
            _active.Remove(item);
        }

        public void ReturnAll()
        {
            foreach (TPoolObject item in _active.ToArray())
            {
                Return(item);
            }
        }
    }
}
