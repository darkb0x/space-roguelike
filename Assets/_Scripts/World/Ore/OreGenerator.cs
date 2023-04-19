using System.Collections.Generic;
using UnityEngine;

namespace Game.World.Generation.Ore
{
    using SaveData;
    using MainMenu.Mission.Planet;

    public class OreGenerator : MonoBehaviour
    {
        [SerializeField, NaughtyAttributes.Expandable] private PlanetSO DefaultPlanet;

        private Ore[] AllOres;
        private PlanetSO LoadedPlanet;

        private void Start()
        {
            LoadedPlanet = GameData.Instance.CurrentSessionData.GetPlanet();
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
