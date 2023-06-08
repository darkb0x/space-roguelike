using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Game.World.Generation.Ore
{
    using SaveData;
    using MainMenu.MissionChoose.Planet;

    public class OreGenerator : MonoBehaviour
    {
        //[SerializeField, ReadOnly] private int OreZones;
        //[SerializeField] private float OreRadius;
        //[SerializeField] private Vector2 ZonesOffset = Vector2.zero;
        //[Space]
        [SerializeField, Expandable] private PlanetSO DefaultPlanet;

        private Ore[] AllOres;
        private PlanetSO LoadedPlanet;

        /*
        private void OnPlanetChanged()
        {
            OreZones = DefaultPlanet.OresInPlanet.Count;
        }

        private void OnDrawGizmosSelected()
        {
            if (DefaultPlanet == null)
                return;

            for (int i = 0; i < DefaultPlanet.OresInPlanet.Count; i++)
            {
                float direction = ((float)i / DefaultPlanet.OresInPlanet.Count) * 360f;
                Vector2 pos = (Vector2)transform.position - ZonesOffset + new Vector2(Mathf.Sin(direction * Mathf.Deg2Rad), Mathf.Cos(direction * Mathf.Deg2Rad)) * OreRadius;
                Debug.DrawLine(transform.position - (Vector3)ZonesOffset, pos, Color.green);
            }
        }
        */

        private void Start()
        {
            LoadedPlanet = SaveDataManager.Instance.CurrentSessionData.GetPlanet();
            if (LoadedPlanet == null)
                LoadedPlanet = DefaultPlanet;

            AllOres = ShuffleList(FindObjectsOfType<Ore>());

            InitializeOres();
        }

        private void InitializeOres()
        {
            PlanetSO.ItemGenerationData[] generationData = new PlanetSO.ItemGenerationData[LoadedPlanet.OresInPlanet.Count];
            Debug.Log("Ore amount = " + AllOres.Length);
            for (int i = 0; i < LoadedPlanet.OresInPlanet.Count; i++)
            {
                PlanetSO.ItemGenerationData data = LoadedPlanet.DefaultItems[i];
                int generatedOreAmount = Mathf.RoundToInt(data.PercentInWorld * AllOres.Length / 100);
                Debug.Log($"Amount of {data.Item.ItemName} ore: {generatedOreAmount}");
                generationData[i] = new PlanetSO.ItemGenerationData() { Item = data.Item, PercentInWorld = generatedOreAmount };
            }

            int initializedOreAmount = 0;
            for (int i = 0; i < generationData.Length; i++)
            {
                for (int j = 0; j < generationData[i].PercentInWorld; j++)
                {
                    AllOres[j + initializedOreAmount].Initialize(generationData[i].Item);
                }
                initializedOreAmount += generationData[i].PercentInWorld;
            }

            for (int i = 0; i < AllOres.Length; i++)
            {
                if (!AllOres[i].isInitialized)
                {
                    AllOres[i].Initialize(generationData[0].Item);
                    initializedOreAmount++;
                }
            }
        }

        public Ore[] ShuffleList(Ore[] inputArray)
        {
            List<Ore> inputList = new List<Ore>();

            foreach (var item in inputArray)
            {
                inputList.Add(item);
            }

            List<Ore> randomList = new List<Ore>();
            System.Random random = new System.Random();

            while (inputList.Count > 0)
            {
                int index = random.Next(0, inputList.Count);
                randomList.Add(inputList[index]);
                inputList.RemoveAt(index);
            }

            return randomList.ToArray();
        }
    }
}
