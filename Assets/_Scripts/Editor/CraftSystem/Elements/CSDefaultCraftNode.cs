using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CraftSystem.Elements
{
    using CraftSystem.ScriptableObjects;
    using Data.Save;
    using Enumerations;
    using System.Collections.Generic;
    using Utilities;
    using Windows;

    public class CSDefaultCraftNode : CSNode
    {
        public GameObject CraftPrefab { get; set; }
        public int CraftCost { get; set; }

        public override void Initialize(string nodeName, CSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            CraftType = CSCraftType.DefaultCraft;

            CSChoiceSaveData choiceData = new CSChoiceSaveData()
            {
                Description = "Next Craft"
            };

            Choices.Add(choiceData);
        }

        public override void LoadData(CSNodeSaveData data, List<CSChoiceSaveData> choices)
        {
            ID = data.ID;
            Choices = choices;
            Description = data.Description;

            CSDefaultCraftSaveData nodeData = data as CSDefaultCraftSaveData;
            CraftPrefab = nodeData.CraftPrefab;
        }

        public override CSTreeCraftSO SaveToSO(string path)
        {
            CSTreeCraftSO so = CSIOUtility.CreateAsset<CSTreeCraftSO>(path, CraftName);

            so.Initialize(
                CraftName,
                Description,
                CSIOUtility.ConvertNodeChoicesToCraftChoices(Choices),
                CraftType,
                CraftPrefab,
                IsStartingNode()
            );

            return so;
        }

        public override CSNodeSaveData ConvertToGraphSaveData()
        {
            CSNodeSaveData nodeData = new CSDefaultCraftSaveData()
            {
                ID = ID,
                Name = CraftName,
                Choices = CSIOUtility.CloneNodeChoices(Choices),
                Description = Description,
                GroupID = Group?.ID,
                CraftType = CraftType,
                Position = GetPosition().position,
                CraftPrefab = CraftPrefab
            };

            return nodeData;
        }

        public override void Draw()
        {
            base.Draw();

            AddOutputContainer();

            RefreshExpandedState();
        }

        private void AddOutputContainer()
        {
            foreach (CSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Description);

                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }
        }

        protected override void AddExtenshionContainer()
        {
            base.AddExtenshionContainer();
        }
    }
}
