using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;
using System;
using System.Linq;

namespace Game.CraftSystem.Editor.Elements
{
    using Utilities;
    using Windows;
    using Data.Save;
    using Player.Inventory;

    public class CSNode : Node
    {
        public string ID { get; set; }
        public string CraftName { get; set; }
        public List<CSChoiceSaveData> Choices { get; set; }
        public GameObject Prefab { get; set; }
        public Sprite Icon { get; set; }
        public int Cost { get; set; }
        public List<ItemCraft> Craft { get; set; }
        public CSGroup Group { get; set; }

        protected CSGraphView graphView;
        private Color defaultBackgroundColor;

        public virtual void Initialize(string nodeName, CSGraphView dsGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();

            CraftName = nodeName;
            Choices = new List<CSChoiceSaveData>();
            Prefab = null;
            Cost = 0;
            Craft = new List<ItemCraft>();

            CSChoiceSaveData choiceData = new CSChoiceSaveData();
            Choices.Add(choiceData);
            Craft.Add(new ItemCraft());

            graphView = dsGraphView;
            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

            SetPosition(new Rect(position, Vector2.zero));

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        public virtual void Draw()
        {
            // Title container
            TextField craftNameTextField = CSElementUtility.CreateTextField(CraftName, null, callback =>
            {
                TextField target = (TextField) callback.target;

                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                if(string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(CraftName))
                    {
                        graphView.NameErrorsAmount++;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(CraftName))
                    {
                        graphView.NameErrorsAmount--;
                    }
                }

                if(Group == null)
                {
                    graphView.RemoveUngroupedNode(this);

                    CraftName = target.value;

                    graphView.AddUngroupedNode(this);

                    return;
                }

                CSGroup currentGroup = Group;

                graphView.RemoveGroupedNode(this, Group);

                CraftName = callback.newValue;

                graphView.AddGroupedNode(this, currentGroup);
            });

            craftNameTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__filename-textfield",
                "ds-node__textfield__hidden"
            );

            titleContainer.Insert(0, craftNameTextField);

            // Input container

            Port inputPort = this.CreatePort("", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputContainer.Add(inputPort);

            // Output container

            foreach (CSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort("Next Craft", capacity: Port.Capacity.Multi);

                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }

            // Data Container
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__custom-data-container");

            // Cost
            IntegerField costField = CSElementUtility.CreateIntField("Turret Cost", (callback) =>
            {
                Cost = callback.newValue;
            });
            costField.value = Cost;

            customDataContainer.Add(costField);

            // Object
            ObjectField turretObjectField = CSElementUtility.CreateObjectField("Turret Object", typeof(GameObject), (callback) =>
            {
                Prefab = (GameObject) callback.newValue;
            });
            turretObjectField.value = Prefab;

            customDataContainer.Add(turretObjectField);

            // Image
            ObjectField turretIconField = CSElementUtility.CreateObjectField("Turret Icon", typeof(Sprite), (callback) =>
            {
                Icon = (Sprite)callback.newValue;
            });
            turretIconField.value = Icon;

            customDataContainer.Add(turretIconField);

            mainContainer.Add(customDataContainer);

            #region Craft field
            VisualElement customDataContainer_Craft = new VisualElement();
            customDataContainer_Craft.AddToClassList("ds-node__custom-data-container");

            Foldout craftFoldout = CSElementUtility.CreateFoldout("Craft");

            for (int i = 0; i < Craft.Count; i++)
            {
                AddElementToFoldout(craftFoldout, Craft[i]);
            }

            //Add Element
            Button AddButton = CSElementUtility.CreateButton("Add", () =>
            {
                Craft.Add(AddElementToFoldout(craftFoldout, new ItemCraft()));
            });
            craftFoldout.Insert(0, AddButton);

            customDataContainer_Craft.Add(craftFoldout);

            extensionContainer.Add(customDataContainer_Craft);
            #endregion

            RefreshExpandedState();
        }

        private ItemCraft AddElementToFoldout(Foldout craftFoldout, ItemCraft craft)
        {
            // Element Foldout
            Foldout elementFoldout = CSElementUtility.CreateFoldout("Item");

            //Item Object
            ObjectField itemField = CSElementUtility.CreateObjectField("Item", typeof(InventoryItem), (callback) =>
            {
                craft.item = (InventoryItem)callback.newValue;
            });
            itemField.value = craft.item;

            //Item Amount
            IntegerField itemAmountField = CSElementUtility.CreateIntField("Amount", (callback) =>
            {
                craft.amount = callback.newValue;
            });
            itemAmountField.value = craft.amount;

            //Remove Button
            Button removeButton = CSElementUtility.CreateButton("Remove", () =>
            {
                if(Craft.Count > 1)
                {
                    RemoveElementFromFoldout(craftFoldout, craft, elementFoldout);
                }
            });

            //Add elements to foldout
            elementFoldout.Add(itemField);
            elementFoldout.Add(itemAmountField);
            elementFoldout.Add(removeButton);

            //Add foldout to main foldout
            craftFoldout.Add(elementFoldout);

            return craft;
        }
        private void RemoveElementFromFoldout(Foldout craftFoldout, ItemCraft craft, VisualElement element)
        {
            Craft.Remove(craft);
            craftFoldout.Remove(element);
        }

        #region Ovverided methods
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconect Input Ports", actionEvent => DisconectInputPorts());
            evt.menu.AppendAction("Disconect Output Ports", actionEvent => DisconectOutputPorts());

            base.BuildContextualMenu(evt);
        }
        #endregion

        #region Utility Methods
        public void DisconectInputPorts()
        {
            DisconectPorts(inputContainer);
        }
        public void DisconectOutputPorts()
        {
            DisconectPorts(outputContainer);
        }
        public void DisconectAllPorts()
        {
            DisconectInputPorts();
            DisconectOutputPorts();
        }
        private void DisconectPorts(VisualElement container)
        {
            foreach(Port port in container.Children())
            {
                if(!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }

        public bool IsStartingNode()
        {
            Port inputPort = (Port) inputContainer.Children().First();

            return !inputPort.connected;
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
        #endregion
    }
}
