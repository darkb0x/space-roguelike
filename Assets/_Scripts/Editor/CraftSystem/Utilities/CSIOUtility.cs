using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Game.CraftSystem.Editor.Utilities
{
    using Elements;
    using Windows;
    using Data.Save;
    using ScriptableObjects;
    using CraftSystem.Editor.Data;

    public static class CSIOUtility
    {
        private static CSGraphView graphView;

        private static string graphFileName;
        private static string containerFolderPath;

        private static List<CSGroup> groups;
        private static List<CSNode> nodes;

        private static Dictionary<string, CSCraftGroupSO> createdDialogueGroups;
        private static Dictionary<string, CSCraftSO> createdDialogues;

        private static Dictionary<string, CSGroup> loadedGroups;
        private static Dictionary<string, CSNode> loadedNodes;

        public static void Initialize(CSGraphView dsGraphView, string graphName)
        {
            graphView = dsGraphView;

            graphFileName = graphName;
            containerFolderPath = $"Assets/Resources/CraftSystem/Crafts/{graphFileName}";

            groups = new List<CSGroup>();
            nodes = new List<CSNode>();

            createdDialogueGroups = new Dictionary<string, CSCraftGroupSO>();
            createdDialogues = new Dictionary<string, CSCraftSO>();

            loadedGroups = new Dictionary<string, CSGroup>();
            loadedNodes = new Dictionary<string, CSNode>();
        }

        #region Save methods
        public static void Save()
        {
            CreateStaticFolders();

            GetElementsFromGraphView();

            CSGraphSaveDataSO graphData = CreateAsset<CSGraphSaveDataSO>("Assets/Resources/CraftSystem/Graphs", $"{graphFileName}Graph");

            graphData.Initialize(graphFileName);

            CSCraftContainerSO dialogueContainer = CreateAsset<CSCraftContainerSO>(containerFolderPath, graphFileName);

            dialogueContainer.Initialize(graphFileName);

            SaveGroups(graphData, dialogueContainer);
            SaveNodes(graphData, dialogueContainer);

            SaveAsset(graphData);
            SaveAsset(dialogueContainer);
        }
        #region Groups
        private static void SaveGroups(CSGraphSaveDataSO graphData, CSCraftContainerSO dialogueContainer)
        {
            List<string> groupNames = new List<string>();

            foreach (CSGroup group in groups)
            {
                SaveGroupToGraph(group, graphData);
                SaveGroupToScriptableObject(group, dialogueContainer);

                groupNames.Add(group.title);
            }

            UpdateOldGroups(groupNames, graphData);
        }

        private static void SaveGroupToGraph(CSGroup group, CSGraphSaveDataSO graphData)
        {
            CSGroupSaveData groupData = new CSGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
            };

            graphData.Groups.Add(groupData);
        }

        private static void SaveGroupToScriptableObject(CSGroup group, CSCraftContainerSO dialogueContainer)
        {
            string groupName = group.title;

            CreateFolder($"{containerFolderPath}/Groups", groupName);
            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Crafts");

            CSCraftGroupSO dialogueGroup = CreateAsset<CSCraftGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);

            dialogueGroup.Initialize(groupName);

            createdDialogueGroups.Add(group.ID, dialogueGroup);

            dialogueContainer.DialogueGroups.Add(dialogueGroup, new List<CSCraftSO>());

            SaveAsset(dialogueGroup);
        }

        private static void UpdateOldGroups(List<string> currentGroupNames, CSGraphSaveDataSO graphData)
        {
            if(graphData.OldGroupNames != null && graphData.OldGroupNames.Count != 0)
            {
                List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

                foreach (string groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
                }
            }

            graphData.OldGroupNames = new List<string>(currentGroupNames);
        }
        #endregion

        #region Nodes
        private static void SaveNodes(CSGraphSaveDataSO graphData, CSCraftContainerSO dialogueContainer)
        {
            SerializableDictionary<string, List<string>> groupedNodeNames = new SerializableDictionary<string, List<string>>();
            List<string> ungroupedNodeNames = new List<string>();

            foreach (CSNode node in nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, dialogueContainer);

                if(node.Group != null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.CraftName);

                    continue;
                }

                ungroupedNodeNames.Add(node.CraftName);
            }

            UpdateDialoguesChoicesConnections();

            UpdateOldGroupedNodes(groupedNodeNames, graphData);
            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
        }

        private static void SaveNodeToGraph(CSNode node, CSGraphSaveDataSO graphData)
        {
            List<CSChoiceSaveData> choices = CloneNodeChoices(node.Choices);

            CSNodeSaveData nodeData = new CSNodeSaveData()
            {
                ID = node.ID,
                Name = node.CraftName,
                Choices = choices,
                Prefab = node.Prefab,
                Icon = node.Icon,
                Cost = node.Cost,
                Craft = node.Craft,
                GroupID = node.Group?.ID,
                Position = node.GetPosition().position
            };

            graphData.Nodes.Add(nodeData);
        }

        private static void SaveNodeToScriptableObject(CSNode node, CSCraftContainerSO dialogueContainer)
        {
            CSCraftSO craft;

            if (node.Group != null)
            {
                craft = CreateAsset<CSCraftSO>($"{containerFolderPath}/Groups/{node.Group.title}/Crafts", node.CraftName);

                dialogueContainer.DialogueGroups.AddItem(createdDialogueGroups[node.Group.ID], craft);
            }
            else
            {
                craft = CreateAsset<CSCraftSO>($"{containerFolderPath}/Global/Crafts", node.CraftName);

                dialogueContainer.UngroupedDialogues.Add(craft);
            }

            craft.Initialize(
                node.CraftName,
                ConvertNodeChoicesToDialogueChoices(node.Choices),
                craft.GameObjectPrefab,
                craft.IconSprite,
                craft.CraftCost,
                craft.ObjectCraft,
                node.IsStartingNode()
            );

            createdDialogues.Add(node.ID, craft);

            SaveAsset(craft);
        }

        private static List<CSCraftChoiceData> ConvertNodeChoicesToDialogueChoices(List<CSChoiceSaveData> nodeChoices)
        {
            List<CSCraftChoiceData> dialogueChoice = new List<CSCraftChoiceData>();

            foreach (CSChoiceSaveData nodeChoice in nodeChoices)
            {
                CSCraftChoiceData choiceData = new CSCraftChoiceData();

                dialogueChoice.Add(choiceData);
            }

            return dialogueChoice;
        }

        private static void UpdateDialoguesChoicesConnections()
        {
            foreach (CSNode node in nodes)
            {
                CSCraftSO dialogue = createdDialogues[node.ID];

                for (int choiceIndex = 0; choiceIndex < node.Choices.Count; choiceIndex++)
                {
                    CSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                    if(string.IsNullOrEmpty(nodeChoice.NodeID))
                    {
                        continue;
                    }

                    dialogue.Choices[choiceIndex].NextDialogue = createdDialogues[nodeChoice.NodeID];

                    SaveAsset(dialogue);
                }
            }
        }

        private static void UpdateOldGroupedNodes(SerializableDictionary<string, List<string>> currentGroupedNodeNames, CSGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
                {
                    List<string> nodesToRemove = new List<string>();

                    if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                    {
                        nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
                    }

                    foreach (string nodeToRemove in nodesToRemove)
                    {
                        RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Crafts", nodeToRemove);
                    }
                }
            }

            graphData.OldGroupedNodeNames = new SerializableDictionary<string, List<string>>(currentGroupedNodeNames);
        }

        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, CSGraphSaveDataSO graphData)
        {
            if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
            {
                List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Global/Crafts", nodeToRemove);
                }
            }

            graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
        }
        #endregion
        #endregion

        #region Load methods 
        public static void Load()
        {
            CSGraphSaveDataSO graphData = LoadAsset<CSGraphSaveDataSO>("Assets/Resources/CraftSystem/Graphs", graphFileName);

            if (graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "Could not find the file!",
                    "The file at the following path could not be found:\n\n" +
                    $"\"Assets/Resources/CraftSystem/Graphs/{graphFileName}\".\n\n" +
                    "Make sure you chose the right file and it's placed at the folder path mentioned above.",
                    "Thanks!"
                );

                return;
            }

            CSEditorWindow.UpdateFileName(graphData.FileName);

            LoadGroups(graphData.Groups);
            LoadNodes(graphData.Nodes);
            LoadNodesConnections();
        }

        private static void LoadGroups(List<CSGroupSaveData> groups)
        {
            foreach(CSGroupSaveData groupData in groups)
            {
                CSGroup group = graphView.CreateGroup(groupData.Name, groupData.Position);

                group.ID = groupData.ID;

                loadedGroups.Add(group.ID, group);
            }
        }

        private static void LoadNodes(List<CSNodeSaveData> nodes)
        {
            foreach (CSNodeSaveData nodeData in nodes)
            {
                List<CSChoiceSaveData> choices = CloneNodeChoices(nodeData.Choices);

                CSNode node = graphView.CreateNode(nodeData.Position, false, nodeData.Name);

                node.ID = nodeData.ID;
                node.Choices = choices;
                node.Prefab = nodeData.Prefab;
                node.Icon = nodeData.Icon;
                node.Cost = nodeData.Cost;
                node.Craft = nodeData.Craft;

                node.Draw();

                graphView.AddElement(node);

                loadedNodes.Add(node.ID, node);

                if(string.IsNullOrEmpty(nodeData.GroupID))
                {
                    continue;
                }

                CSGroup group = loadedGroups[nodeData.GroupID];

                node.Group = group;

                group.AddElement(node);
            }
        }

        private static void LoadNodesConnections()
        {
            foreach (KeyValuePair<string, CSNode> loadedNode in loadedNodes)
            {
                foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
                {
                    CSChoiceSaveData choiceData = (CSChoiceSaveData)choicePort.userData;

                    if (string.IsNullOrEmpty(choiceData.NodeID))
                    {
                        continue;
                    }

                    CSNode nextNode = loadedNodes[choiceData.NodeID];

                    Port nextNodeInputPort = (Port)nextNode.inputContainer.Children().First();

                    Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                    graphView.AddElement(edge);

                    loadedNode.Value.RefreshPorts();
                }
            }
        }
        #endregion

        #region Creation methods
        private static void CreateStaticFolders()
        {
            CreateFolder("Assets/Resources/CraftSystem", "Crafts");
            CreateFolder("Assets/Resources/CraftSystem", "Graphs");

            CreateFolder("Assets/Resources/CraftSystem/Crafts", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder($"{containerFolderPath}/Global", "Crafts");
        }
        #endregion

        #region Fetch methods 
        private static void GetElementsFromGraphView()
        {
            Type groupType = typeof(CSGroup);

            graphView.graphElements.ForEach(graphElement =>
            {
                if(graphElement is CSNode node)
                {
                    nodes.Add(node);

                    return;
                }

                if(graphElement.GetType() == groupType)
                {
                    CSGroup group = (CSGroup)graphElement;

                    groups.Add(group);

                    return;
                }
            });
        }
        #endregion

        #region Utility methods
        private static void CreateFolder(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder($"{path}/{folderName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(path, folderName);
        }
        private static void RemoveFolder(string fullPath)
        {
            FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
            FileUtil.DeleteFileOrDirectory($"{fullPath}/");
        }

        private static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, fullPath);
            }

            return asset;
        }

        private static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";


            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        private static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }
        private static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static List<CSChoiceSaveData> CloneNodeChoices(List<CSChoiceSaveData> nodeChoices)
        {
            List<CSChoiceSaveData> choices = new List<CSChoiceSaveData>();

            foreach (CSChoiceSaveData choice in nodeChoices)
            {
                CSChoiceSaveData choiceData = new CSChoiceSaveData()
                {
                    NodeID = choice.NodeID
                };

                choices.Add(choiceData);
            }

            return choices;
        }
        #endregion
    }
}
