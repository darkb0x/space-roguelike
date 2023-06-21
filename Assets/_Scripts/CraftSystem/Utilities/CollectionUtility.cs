using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

namespace CraftSystem.Utilities
{
    public static class CollectionUtility
    {
        public static void AddItem<T, K>(this SerializedDictionary<T, List<K>> serializableDictionary, T key, K value)
        {
            if (serializableDictionary.ContainsKey(key))
            {
                serializableDictionary[key].Add(value);

                return;
            }

            serializableDictionary.Add(key, new List<K>() { value });
        }
    }
}