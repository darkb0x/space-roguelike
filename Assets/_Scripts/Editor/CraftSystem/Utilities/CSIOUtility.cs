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
        private static string containerFolderPath_ForLoad;

        private static List<CSNode> nodes;

        private static Dictionary<string, CSCraftSO> createdNodes;

        private static Dictionary<string, CSNode> loadedNodes;

        public static void Initialize(CSGraphView dsGraphView, string graphName)
        {
            graphView = dsGraphView;

            graphFileName = graphName;
            containerFolderPath = $"Assets/Resources/CraftSystem/Crafts/{graphFileName}";
            containerFolderPath_ForLoad = $"CraftSystem/Crafts/{graphFileName}";

            nodes = new List<CSNode>();

            createdNodes = new Dictionary<string, CSCraftSO>();

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

            SaveNodes(graphData, dialogueContainer);

            SaveAsset(graphData);
            SaveAsset(dialogueContainer);
        }

        #region Nodes
        private static void SaveNodes(CSGraphSaveDataSO graphData, CSCraftContainerSO dialogueContainer)
        {
            List<string> ungroupedNodeNames = new List<string>();

            foreach (CSNode node in nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, dialogueContainer);

                ungroupedNodeNames.Add(node.CraftName);
            }
            foreach (CSNode node in nodes)
            {
                CSCraftSO craft = createdNodes[node.ID];

                foreach (var ChoiceSaveData in node.Choices)
                {
                    foreach (var SOchoiceData in craft.Choices)
                    {
                        if(!string.IsNullOrEmpty(ChoiceSaveData.NodeID)) SOchoiceData.NextCraft = createdNodes[ChoiceSaveData.NodeID];
                    }
                }
            }

            UpdateDialoguesChoicesConnections();

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
                Position = node.GetPosition().position
            };

            graphData.Nodes.Add(nodeData);
        }

        private static void SaveNodeToScriptableObject(CSNode node, CSCraftContainerSO dialogueContainer)
        {
            CSCraftSO craft;

            craft = CreateAsset<CSCraftSO>($"{containerFolderPath}/Nodes", node.CraftName);

            dialogueContainer.Nodes.Add(craft);

            craft.Initialize(
                node.CraftName,
                ConvertNodeChoicesToCraftChoices(node.Choices),
                node.Prefab,
                node.Icon,
                node.Cost,
                node.Craft,
                node.IsStartingNode(),
                node.GetPosition().position,
                $"{containerFolderPath_ForLoad}/Nodes/{node.CraftName}"
            );

            createdNodes.Add(node.ID, craft);

            SaveAsset(craft);
        }

        private static List<CSCraftChoiceData> ConvertNodeChoicesToCraftChoices(List<CSChoiceSaveData> nodeChoices)
        {
            List<CSCraftChoiceData> craftChoice = new List<CSCraftChoiceData>(nodeChoices.Count);
            
            foreach (CSChoiceSaveData nodeChoice in nodeChoices)
            {
                if (string.IsNullOrEmpty(nodeChoice.NodeID))
                    continue;

                CSCraftChoiceData choiceData = new CSCraftChoiceData();

                craftChoice.Add(choiceData);
            }

            return craftChoice;
        }

        private static void UpdateDialoguesChoicesConnections()
        {
            foreach (CSNode node in nodes)
            {
                CSCraftSO dialogue = createdNodes[node.ID];

                for (int choiceIndex = 0; choiceIndex < node.Choices.Count; choiceIndex++)
                {
                    CSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                    if(string.IsNullOrEmpty(nodeChoice.NodeID))
                    {
                        continue;
                    }

                    dialogue.Choices[choiceIndex].NextCraft = createdNodes[nodeChoice.NodeID];

                    SaveAsset(dialogue);
                }
            }
        }

        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, CSGraphSaveDataSO graphData)
        {
            if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
            {
                List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Nodes", nodeToRemove);
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

            LoadNodes(graphData.Nodes);
            LoadNodesConnections();
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
            CreateFolder(containerFolderPath, "Nodes");
        }
        #endregion

        #region Fetch methods 
        private static void GetElementsFromGraphView()
        {
            graphView.graphElements.ForEach(graphElement =>
            {
                if(graphElement is CSNode node)
                {
                    nodes.Add(node);

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
