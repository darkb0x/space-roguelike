using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CraftSystem.Windows
{
    using Elements;
    using Enumerations;

    public class CSSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private CSGraphView graphView;
        private Texture2D indentationIcon;

        public void Initialize(CSGraphView dsGraphView)
        {
            graphView = dsGraphView;

            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, Color.clear);
            indentationIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements")),
                new SearchTreeGroupEntry(new GUIContent("Craft Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Default Craft", indentationIcon))
                {
                    userData = CSCraftType.DefaultCraft,
                    level = 2
                },
                new SearchTreeGroupEntry(new GUIContent("Craft Groups"), 1),
                new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
                {
                    userData = new Group(),
                    level = 2
                }
            };

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);

            switch (SearchTreeEntry.userData)
            {
                case CSCraftType.DefaultCraft:
                {
                    CSDefaultCraftNode singleChoiceNode = (CSDefaultCraftNode) graphView.CreateNode("DialogueName", CSCraftType.DefaultCraft, localMousePosition);

                    graphView.AddElement(singleChoiceNode);

                    return true;
                }

                case Group _:
                {
                    graphView.CreateGroup("DialogueGroup", localMousePosition);

                    return true;
                }

                default:
                {
                    return false;
                }
            }
        }
    }
}