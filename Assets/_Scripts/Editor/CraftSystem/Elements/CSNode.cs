using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CraftSystem.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;
    using ScriptableObjects;

    public abstract class CSNode : Node
    {
        public string ID { get; set; }
        public List<string> OutputIDs { get; set; }
        public string CraftName { get; set; }
        public List<CSChoiceSaveData> Choices { get; set; }
        public string Description { get; set; }
        public CSCraftType CraftType { get; set; }
        public CSGroup Group { get; set; }

        protected CSGraphView graphView;
        private Color defaultBackgroundColor;

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Reset Position", actionEvent => SetPosition(new Rect(Vector2.zero, Vector2.zero)));
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());

            base.BuildContextualMenu(evt);
        }

        public virtual void Initialize(string nodeName, CSGraphView dsGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            OutputIDs = new List<string>();

            CraftName = nodeName;
            Choices = new List<CSChoiceSaveData>();
            Description = "Craft description.";

            SetPosition(new Rect(position, Vector2.zero));

            graphView = dsGraphView;
            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        public abstract void LoadData(CSNodeSaveData data, List<CSChoiceSaveData> choices);
        public abstract CSTreeCraftSO SaveToSO(string path);
        public abstract CSNodeSaveData ConvertToGraphSaveData();

        public virtual void Draw()
        {
            AddTitleContainer();
            AddInputContainer();
            AddExtenshionContainer();
        }

        protected virtual void AddTitleContainer()
        {
            TextField craftNameTextField = CSElementUtility.CreateTextField(CraftName, null, callback =>
            {
                TextField target = (TextField)callback.target;

                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(CraftName))
                    {
                        ++graphView.NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(CraftName))
                    {
                        --graphView.NameErrorsAmount;
                    }
                }

                if (Group == null)
                {
                    graphView.RemoveUngroupedNode(this);

                    CraftName = target.value;

                    graphView.AddUngroupedNode(this);

                    return;
                }

                CSGroup currentGroup = Group;

                graphView.RemoveGroupedNode(this, Group);

                CraftName = target.value;

                graphView.AddGroupedNode(this, currentGroup);
            });

            craftNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__filename-text-field"
            );

            titleContainer.Insert(0, craftNameTextField);
        }

        protected virtual void AddInputContainer()
        {
            Port inputPort = this.CreatePort("Previous Craft", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputContainer.Add(inputPort);
        }

        protected virtual void AddExtenshionContainer()
        {
            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            Foldout textFoldout = CSElementUtility.CreateFoldout("Craft Description", true);

            TextField textTextField = CSElementUtility.CreateTextArea(Description, null, callback => Description = callback.newValue);

            textTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__quote-text-field"
            );

            textFoldout.Add(textTextField);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);
        }

        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }

        public bool IsStartNode()
        {
            Port inputPort = (Port)inputContainer.Children().First();

            return !inputPort.connected;
        }
        public bool IsStartingNodeInGroup()
        {
            if (Group == null)
            {
                return false;
            }
            else
            {
                if (OutputIDs == null | OutputIDs.Count == 0)
                    return true;
                else
                {
                    foreach (var id in OutputIDs)
                    {
                        if (Group.NodeIDs.Contains(id))
                            return false;
                    }
                    return true;
                }
            }
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }
    }
}