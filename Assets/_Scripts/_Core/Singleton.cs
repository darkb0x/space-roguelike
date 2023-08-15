using System.Collections.Generic;
using System;
using UnityEngine;

namespace Game
{
    public interface ISingleton
    {
        public object GetSingleton()
        {
            return this;
        }
    }

    public class Singleton : MonoBehaviour
    {
        public static Singleton Instance { get; private set; }
        [SerializeReference] private static List<ISingleton> singletones;

        private void Awake()
        {
            Instance = this;
            singletones = new List<ISingleton>();
        }

        public static void Add(ISingleton singleton)
        {
            if(!singletones.Contains(singleton))
            {
                singletones.Add(singleton);
            }
        }

        public static T Get<T>() where T : class
        {
            foreach (var singleton in singletones)
            {
                if(singleton.GetType() == typeof(T))
                {
                    return (T)singleton.GetSingleton();
                }
            }
            return default(T);
        }
    }
}
