using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;

namespace Game.CraftSystem.Editor.Windows
{
    using Data.Save;
    using Data.Error;
    using Elements;
    using Utilities;

    public class CSGraphView : GraphView
    {
        private CSEditorWindow editorWindow;
        private CSSearchWindow searchWindow;

        private MiniMap miniMap;

        private SerializableDictionary<string, CSNodeErrorData> ungroupedNodes;
        private SerializableDictionary<string, CSGroupErrorData> groups;
        private SerializableDictionary<Group, SerializableDictionary<string, CSNodeErrorData>> groupedNodes;

        private int nameErrorsAmount;

        public int NameErrorsAmount
        {
            get
            {
                return nameErrorsAmount;
            }
            set
            {
                nameErrorsAmount = value;

                if(nameErrorsAmount == 0)
                {
                    editorWindow.EnableSaving();
                }

                if (nameErrorsAmount == 1)
                {
                    editorWindow.DisableSaving();
                }
            }
        }

        public CSGraphView(CSEditorWindow dsEditorWindow)
        {
            editorWindow = dsEditorWindow;

            ungroupedNodes = new SerializableDictionary<string, CSNodeErrorData>();
            groups = new SerializableDictionary<string, CSGroupErrorData>();
            groupedNodes = new SerializableDictionary<Group, SerializableDictionary<string, CSNodeErrorData>>();

            AddManipulators();
            AddSearchWindow();
            AddMinimap();
            AddGridBackground();

            OnElementsDeleted();
            OnGroupElementAdded();
            OnGroupElementRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();

            AddStyles();
            AddMiniMapStyles();
        }

        #region Overrided methods
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if(startPort == port)
                {
                    return;
                }

                if(startPort.node == port.node)
                {
                    return;
                }

                if(startPort.direction == port.direction)
                {
                    return;
                }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }
        #endregion

        #region Manipulators
        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu("Add Node"));

            this.AddManipulator(CreateGroupContextualMenu());
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => CreateGroup("Dialogue Group", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );

            return contextualMenuManipulator;
        }
        #endregion

        #region Elements creation
        public CSGroup CreateGroup(string title, Vector2 localMousePosition)
        {
            CSGroup group = new CSGroup(title, localMousePosition);

            AddGroup(group);

            AddElement(group);

            foreach (GraphElement selectedElement in selection)
            {
                if(!(selectedElement is CSNode))
                {
                    continue;
                }

                CSNode node = (CSNode) selectedElement;

                group.AddElement(node);
            }

            return group;
        }

        public CSNode CreateNode(Vector2 position, bool shouldDraw = true, string nodeName = "CraftName")
        {
            Type nodeType = Type.GetType($"CSNode");

            CSNode node = new CSNode();

            node.Initialize(nodeName, this, position);

            if(shouldDraw)
                node.Draw();

            AddUngroupedNode(node);

            return node;
        }
        #endregion

        #region Callbacks
        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type groupType = typeof(CSGroup);
                Type edgeType = typeof(Edge);

                List<CSGroup> groupsToDelete = new List<CSGroup>();
                List<Edge> edgesToDelete = new List<Edge>();
                List<CSNode> nodesToDelete = new List<CSNode>();

                foreach (GraphElement element in selection)
                {
                    if(element is CSNode node)
                    {
                        nodesToDelete.Add(node);

                        continue;
                    }

                    if(element.GetType() == edgeType)
                    {
                        Edge edge = (Edge)element;

                        edgesToDelete.Add(edge);

                        continue;
                    }

                    if(element.GetType() != groupType)
                    {
                        continue;
                    }

                    CSGroup group = (CSGroup)element;

                    groupsToDelete.Add(group);
                }

                foreach (CSGroup group in groupsToDelete)
                {
                    List<CSNode> groupNodes = new List<CSNode>();

                    foreach (GraphElement groupElement in group.containedElements)
                    {
                        if(!(groupElement is CSNode))
                        {
                            continue;
                        }

                        CSNode groupNode = (CSNode)groupElement;

                        groupNodes.Add(groupNode);
                    }

                    group.RemoveElements(groupNodes);

                    RemoveGroup(group);

                    RemoveElement(group);
                }

                DeleteElements(edgesToDelete);

                foreach (CSNode node in nodesToDelete)
                {
                    if(node.Group != null)
                    {
                        node.Group.RemoveElement(node);
                    }

                    RemoveUngroupedNode(node);

                    node.DisconectAllPorts();

                    RemoveElement(node);
                }
            };
        }

        private void OnGroupElementAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if(!(element is CSNode))
                    {
                        continue;
                    }

                    CSGroup nodeGroup = (CSGroup) group;
                    CSNode node = (CSNode) element;

                    RemoveUngroupedNode(node);

                    AddGroupedNode(node, nodeGroup);
                }
            };
        }

        private void OnGroupElementRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is CSNode))
                    {
                        continue;
                    }

                    CSNode node = (CSNode)element;

                    RemoveGroupedNode(node, group);
                    AddUngroupedNode(node);
                }
            };
        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                CSGroup dsGroup = (CSGroup)group;

                dsGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(dsGroup.title))
                {
                    if (!string.IsNullOrEmpty(dsGroup.OldTitle))
                    {
                        NameErrorsAmount++;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(dsGroup.OldTitle))
                    {
                        NameErrorsAmount--;
                    }
                }

                RemoveGroup(dsGroup);

                dsGroup.OldTitle = dsGroup.title;

                AddGroup(dsGroup);
            };
        }

        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                if(changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        CSNode nextNode = (CSNode) edge.input.node;

                        CSChoiceSaveData choiceData = (CSChoiceSaveData) edge.output.userData;

                        choiceData.NodeID = nextNode.ID;
                    }
                }

                if(changes.elementsToRemove != null)
                {
                    Type edgeType = typeof(Edge);

                    foreach (GraphElement element in changes.elementsToRemove)
                    {
                        if(element.GetType() != edgeType)
                        {
                            continue;
                        }

                        Edge edge = (Edge)element;

                        CSChoiceSaveData choiceData = (CSChoiceSaveData)edge.output.userData;

                        choiceData.NodeID = "";
                    }
                } 

                return changes;
            };
        }
        #endregion

        #region Repeated elements
        public void AddUngroupedNode(CSNode node)
        {
            string nodeName = node.CraftName.ToLower();

            if(!ungroupedNodes.ContainsKey(nodeName))
            {
                CSNodeErrorData nodeErrorData = new CSNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                ungroupedNodes.Add(nodeName, nodeErrorData);

                return;
            }

            List<CSNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Add(node);

            Color errorColor = ungroupedNodes[nodeName].ErrorData.Color;

            node.SetErrorStyle(errorColor);

            if(ungroupedNodesList.Count == 2)
            {
                NameErrorsAmount++;
                ungroupedNodesList[0].SetErrorStyle(errorColor);
            }
        }
        public void RemoveUngroupedNode(CSNode node)
        {
            string nodeName = node.CraftName.ToLower();

            List<CSNode> ungroupedNodesList = ungroupedNodes[nodeName].Nodes;

            ungroupedNodesList.Remove(node);

            node.ResetStyle();

            if (ungroupedNodesList.Count == 1)
            {
                NameErrorsAmount--;
                ungroupedNodesList[0].ResetStyle();

                return;
            }

            if (ungroupedNodesList.Count == 0)
            {
                ungroupedNodes.Remove(nodeName);
            }
        }


        public void AddGroup(CSGroup group)
        {
            string groupName = group.title.ToLower();

            if(!groups.ContainsKey(groupName))
            {
                CSGroupErrorData groupErrorData = new CSGroupErrorData();

                groupErrorData.Groups.Add(group);

                groups.Add(groupName, groupErrorData);

                return;
            }

            List<CSGroup> groupsList = groups[groupName].Groups;

            groupsList.Add(group);

            Color errorColor = groups[groupName].ErrorData.Color;

            group.SetErrorStyle(errorColor);

            if(groupsList.Count == 2)
            {
                NameErrorsAmount++;
                groupsList[0].SetErrorStyle(errorColor);
            }
        }
        public void RemoveGroup(CSGroup group)
        {
            string oldGroupName = group.OldTitle.ToLower();

            List<CSGroup> groupsList = groups[oldGroupName].Groups;

            groupsList.Remove(group);

            group.ResetStyle();

            if(groupsList.Count == 1)
            {
                NameErrorsAmount--;
                groupsList[0].ResetStyle();

                return;
            }

            if(groupsList.Count == 0)
            {
                groups.Remove(oldGroupName);
            }
        }

        public void AddGroupedNode(CSNode node, CSGroup group)
        {
            string nodeName = node.CraftName.ToLower();

            node.Group = group;

            if(!groupedNodes.ContainsKey(group))
            {
                groupedNodes.Add(group, new SerializableDictionary<string, CSNodeErrorData>());
            }

            if (!groupedNodes[group].ContainsKey(nodeName))
            {
                CSNodeErrorData nodeErrorData = new CSNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                groupedNodes[group].Add(nodeName, nodeErrorData);

                return;
            }

            List<CSNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Add(node);

            Color errorColor = groupedNodes[group][nodeName].ErrorData.Color;

            node.SetErrorStyle(errorColor);

            if(groupedNodesList.Count == 2)
            {
                NameErrorsAmount++;
                groupedNodesList[0].SetErrorStyle(errorColor);
            }
        }
        public void RemoveGroupedNode(CSNode node, Group group)
        {
            string nodeName = node.CraftName.ToLower();

            node.Group = null;

            List<CSNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Remove(node);

            node.ResetStyle();

            if(groupedNodesList.Count == 1)
            {
                NameErrorsAmount--;
                groupedNodesList[0].ResetStyle();

                return;
            }

            if(groupedNodesList.Count == 0)
            {
                groupedNodes[group].Remove(nodeName);

                if (groupedNodes[group].Count == 0)
                {
                    groupedNodes.Remove(group);
                }
            }
        }
        #endregion

        #region Elements addition
        private void AddSearchWindow()
        {
            if(searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<CSSearchWindow>();

                searchWindow.Initialize(this);
            }

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        private void AddMinimap()
        {
            miniMap = new MiniMap()
            {
                anchored = true,

            };

            miniMap.SetPosition(new Rect(15, 50, 200, 180));

            Add(miniMap);

            miniMap.visible = false;
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        private void AddStyles()
        {
            this.AddStyleSheets(
                "DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSNodeStyles.uss"
            );
        }
        private void AddMiniMapStyles()
        {
            StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
            StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

            miniMap.style.backgroundColor = backgroundColor;
            miniMap.style.borderTopColor = borderColor;
            miniMap.style.borderRightColor = borderColor;
            miniMap.style.borderBottomColor = borderColor;
            miniMap.style.borderLeftColor = borderColor;
        }
        #endregion

        #region Utilities
        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition -= editorWindow.position.position;
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }

        public void ClearGraph()
        {
            DeleteElements(graphElements.ToList());

            groups.Clear();
            groupedNodes.Clear();
            ungroupedNodes.Clear();

            NameErrorsAmount = 0;
        }

        public void ToggleMiniMap()
        {
            miniMap.visible = !miniMap.visible;
        }
        #endregion
    }
}
