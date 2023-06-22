using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace CraftSystem.Elements
{
    using Game.Player.Inventory;
    using CraftSystem.ScriptableObjects;
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;
    using static Codice.CM.Common.CmCallContext;
    using System.Linq;

    public class CSDefaultCraftNode : CSNode
    {
        public GameObject CraftPrefab { get; set; }
        public int CraftCost { get; set; }
        public List<ItemData> ItemsInCraft { get; set; }

        private List<Foldout> ItemsFoldouts { get; set; }

        public override void Initialize(string nodeName, CSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            CraftType = CSCraftType.DefaultCraft;
            ItemsInCraft = new List<ItemData>();
            ItemsFoldouts = new List<Foldout>();

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
            CraftCost = nodeData.CraftCost;
            ItemsInCraft = CloneItemData(nodeData.ItemsInCraft);
        }

        public override CSTreeCraftSO SaveToSO(string path)
        {
            CSTreeCraftSO so = CSIOUtility.CreateAsset<CSTreeCraftSO>(path, CraftName);
            Rect rectPosition = GetPosition();
            Vector2 position = new Vector2(rectPosition.x, rectPosition.y);          

            so.Initialize(
                CraftName,
                Description,
                CraftCost,
                CraftType,
                CraftPrefab,
                CloneItemData(ItemsInCraft),
                IsStartingNode(),
                CSIOUtility.ConvertNodeChoicesToCraftChoices(Choices),
                position
            );

            return so;
        }

        public override CSNodeSaveData ConvertToGraphSaveData()
        {
            CSDefaultCraftSaveData nodeData = new CSDefaultCraftSaveData()
            {
                ID = ID,
                Name = CraftName,
                Choices = CSIOUtility.CloneNodeChoices(Choices),
                Description = Description,
                GroupID = Group?.ID,
                CraftType = CraftType,
                Position = GetPosition().position,
                CraftPrefab = CraftPrefab,
                CraftCost = CraftCost,
                ItemsInCraft = CloneItemData(ItemsInCraft)
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
            VisualElement craftObjectVisualElement = new VisualElement();
            craftObjectVisualElement.AddToClassList("ds-node__custom-data-container");

            ObjectField craftPrefabObjectField = CSElementUtility.CreateObjectField("CraftPrefab", typeof(GameObject), false, callback =>
            {
                CraftPrefab = (GameObject)callback.newValue;
            });
            craftPrefabObjectField.value = CraftPrefab;

            IntegerField craftCostIntField = CSElementUtility.CreateIntField("Cost: ", callback =>
            {
                CraftCost = callback.newValue;
            });
            craftCostIntField.value = CraftCost;

            craftObjectVisualElement.Add(craftPrefabObjectField);
            craftObjectVisualElement.Add(craftCostIntField);

            extensionContainer.Add(craftObjectVisualElement);

            base.AddExtenshionContainer();

            VisualElement craftDataContainer = new VisualElement();
            craftDataContainer.AddToClassList("ds-node__custom-data-container");

            Foldout craftFoldout = CSElementUtility.CreateFoldout("Craft");

            Button addItemButton = CSElementUtility.CreateButton("+");
            addItemButton.clicked += () => {
                ItemData newItemData = new ItemData(null);
                Foldout current = CreateItemFoldout(craftFoldout, newItemData, ItemsInCraft.Count);

                craftFoldout.Add(current);
                ItemsFoldouts.Add(current);
                ItemsInCraft.Add(newItemData);

                craftFoldout.Insert(craftFoldout.childCount, addItemButton);
                RefreshExpandedState();
            };

            for (int i = 0; i < ItemsInCraft.Count; i++)
            {
                Foldout current = CreateItemFoldout(craftFoldout, ItemsInCraft[i], i);
                craftFoldout.Add(current);
                ItemsFoldouts.Add(current);
            }

            craftFoldout.Add(addItemButton);

            craftDataContainer.Add(craftFoldout);

            extensionContainer.Add(craftDataContainer);
        }

        private Foldout CreateItemFoldout(VisualElement parent, ItemData itemData, int index)
        {
            Foldout itemFoldout = CSElementUtility.CreateFoldout("Element " + index);

            ObjectField itemObjectField = CSElementUtility.CreateObjectField("Item", typeof(InventoryItem), false, callback =>
            {
                itemData.Item = (InventoryItem)callback.newValue;
            });
            itemObjectField.value = itemData.Item;

            IntegerField craftCostIntField = CSElementUtility.CreateIntField("Amount: ", callback =>
            {
                itemData.Amount = callback.newValue;
            });
            craftCostIntField.value = itemData.Amount;

            Button removeCraftButton = CSElementUtility.CreateButton("-", () =>
            {
                ItemsInCraft.Remove(itemData);
                ItemsFoldouts.Remove(itemFoldout);
                parent.Remove(itemFoldout);
                RefreshElementsInItemsFoldouts();
            });

            itemFoldout.Add(itemObjectField);
            itemFoldout.Add(craftCostIntField);
            itemFoldout.Add(removeCraftButton);

            return itemFoldout;
        }

        private List<ItemData> CloneItemData(List<ItemData> original)
        {
            List<ItemData> clonned = new List<ItemData>();

            foreach (var item in original)
            {
                clonned.Add(new ItemData(item.Item, item.Amount));
            }

            return clonned;
        }

        private void RefreshElementsInItemsFoldouts()
        {
            for (int i = 0; i < ItemsFoldouts.Count; i++)
            {
                Foldout item = ItemsFoldouts[i];
                item.text = "Element " + i;
            }
            RefreshExpandedState();
        }
    }
}
