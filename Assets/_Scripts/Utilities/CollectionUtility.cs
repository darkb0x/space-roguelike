using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

namespace Game.Utilities
{
    public static class CollectionUtility
    {
        public static void AddItem<K, V>(this SerializedDictionary<K, List<V>> serializableDictionary, K key, V value)
        {
            if (serializableDictionary.ContainsKey(key))
            {
                serializableDictionary[key].Add(value);

                return;
            }

            serializableDictionary.Add(key, new List<V>() { value });
        }
    }
}

