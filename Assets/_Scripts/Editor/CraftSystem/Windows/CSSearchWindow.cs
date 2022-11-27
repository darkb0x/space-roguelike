using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Game.CraftSystem.Editor.Windows
{
    using Elements;

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
                new SearchTreeGroupEntry(new GUIContent("Create Element")),

                new SearchTreeGroupEntry(new GUIContent("Craft Nodes"), 1),
                new SearchTreeEntry(new GUIContent("Standart Craft", indentationIcon))
                {
                    level = 2,
                    userData = new CSNode()
                },

                new SearchTreeGroupEntry(new GUIContent("Diologue Group"), 1),
                new SearchTreeEntry(new GUIContent("Single Group", indentationIcon))
                {
                    level = 2,
                    userData = new Group()
                }
            };

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = graphView.GetLocalMousePosition(context.screenMousePosition, true);
            switch(SearchTreeEntry.userData)
            {
                case CSNode _:
                    CSNode node = graphView.CreateNode(localMousePosition);
                    graphView.AddElement(node);
                    return true;

                case Group _:
                    graphView.CreateGroup("DialogueGroup", localMousePosition);
                    return true;

                default:
                    return false;
            }
        }
    }
}
