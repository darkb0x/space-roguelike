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
    using ScriptableObjects;

    public class CSNode : Node
    {
        public string ID { get; set; }
        public string CraftName { get; set; }
        public List<CSChoiceSaveData> Choices { get; set; }
        public GameObject Object { get; set; }
        public Sprite Icon { get; set; }
        public int Cost { get; set; }
        public List<ItemData> Craft { get; set; }

        protected CSGraphView graphView;
        private Color defaultBackgroundColor;

        public virtual void Initialize(string nodeName, CSGraphView dsGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();

            CraftName = nodeName;
            Choices = new List<CSChoiceSaveData>();
            Object = null;
            Cost = 0;
            Craft = new List<ItemData>();

            Craft.Add(new ItemData(null, 1));
            Choices.Add(new CSChoiceSaveData());

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

                graphView.RemoveUngroupedNode(this);

                CraftName = target.value;

                graphView.AddUngroupedNode(this);
            });

            craftNameTextField.AddClasses(
                "ds-node__textfield",
                "ds-node__filename-textfield",
                "ds-node__textfield__hidden"
            );

            titleContainer.Insert(0, craftNameTextField);

            #region Input Container
            Port inputPort = this.CreatePort("", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputContainer.Add(inputPort);
            #endregion

            #region Outup Container
            Button addChoiceButton = CSElementUtility.CreateButton("Add Choice", () =>
            {
                CSChoiceSaveData choiceData = new CSChoiceSaveData();

                Choices.Add(choiceData);

                Port choicePort = CreateChoicePort(choiceData);

                outputContainer.Add(choicePort);
            });

            addChoiceButton.AddToClassList("ds-node_button");

            titleContainer.Add(addChoiceButton);

            foreach (CSChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);

                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }
            #endregion

            #region Data Container
            VisualElement customDataContainer = new VisualElement();
            customDataContainer.AddToClassList("ds-node__custom-data-container");

            // Cost
            IntegerField costField = CSElementUtility.CreateIntField("Cost", (callback) =>
            {
                Cost = callback.newValue;
            });
            costField.value = Cost;

            customDataContainer.Add(costField);

            // Object
            ObjectField turretObjectField = CSElementUtility.CreateObjectField("Object", typeof(GameObject), (callback) =>
            {
                Object = (GameObject) callback.newValue;
            });
            turretObjectField.value = Object;

            customDataContainer.Add(turretObjectField);

            // Image
            ObjectField turretIconField = CSElementUtility.CreateObjectField("Icon", typeof(Sprite), (callback) =>
            {
                Icon = (Sprite)callback.newValue;
            });
            turretIconField.value = Icon;

            customDataContainer.Add(turretIconField);

            mainContainer.Add(customDataContainer);

            #region Craft field
            VisualElement customDataContainer_Craft = new VisualElement();
            customDataContainer_Craft.AddToClassList("ds-node__custom-data-container");

            Foldout craftFoldout = CSElementUtility.CreateFoldout("Craft", true);

            for (int i = 0; i < Craft.Count; i++)
            {
                AddElementToFoldout(craftFoldout, Craft[i]);
            }

            //Add Element
            Button AddButton = CSElementUtility.CreateButton("Add", () =>
            {
                Craft.Add(AddElementToFoldout(craftFoldout, new ItemData(null, 1)));
            });
            craftFoldout.Insert(0, AddButton);

            customDataContainer_Craft.Add(craftFoldout);

            extensionContainer.Add(customDataContainer_Craft);
            #endregion
            #endregion

            RefreshExpandedState();
        }

        private ItemData AddElementToFoldout(Foldout craftFoldout, ItemData craft)
        {
            // Element Foldout
            Foldout elementFoldout = CSElementUtility.CreateFoldout("Item");

            //Item Object
            ObjectField itemField = CSElementUtility.CreateObjectField("Item", typeof(InventoryItem), (callback) =>
            {
                craft.Item = (InventoryItem)callback.newValue;
            });
            itemField.value = craft.Item;

            //Item Amount
            IntegerField itemAmountField = CSElementUtility.CreateIntField("Amount", (callback) =>
            {
                craft.Amount = callback.newValue;
            });
            itemAmountField.value = craft.Amount;

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
        private void RemoveElementFromFoldout(Foldout craftFoldout, ItemData craft, VisualElement element)
        {
            Craft.Remove(craft);
            craftFoldout.Remove(element);
        }

        #region Ovverided methods
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconect Input Ports", actionEvent => DisconectInputPorts());
            evt.menu.AppendAction("Disconect Output Ports", actionEvent => DisconectOutputPorts());

            evt.menu.AppendAction("Reset Position", actionEvent => ResetPositionToZero());

            base.BuildContextualMenu(evt);
        }
        #endregion

        #region Utility Methods
        private Port CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort("Next Craft", capacity: Port.Capacity.Single);

            choicePort.userData = userData;

            CSChoiceSaveData choiceData = (CSChoiceSaveData)userData;

            Button deleteChoiceButton = CSElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count <= 1)
                    return;

                if (choicePort.connected)
                {
                    graphView.DeleteElements(choicePort.connections);
                }

                Choices.Remove(choiceData);

                graphView.RemoveElement(choicePort);
            });

            deleteChoiceButton.AddToClassList("ds-node_button");

            choicePort.Add(deleteChoiceButton);

            return choicePort;
        }

        private void ResetPositionToZero()
        {
            SetPosition(new Rect(Vector2.zero, Vector2.zero));
        }

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
