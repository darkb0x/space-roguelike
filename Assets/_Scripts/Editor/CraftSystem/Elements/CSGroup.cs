using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CraftSystem.Elements
{
    public class CSGroup : Group
    {
        public string ID { get; set; }
        public List<string> NodeIDs { get; set; }
        public string OldTitle { get; set; }

        private Color defaultBorderColor;
        private float defaultBorderWidth;

        public CSGroup(string groupTitle, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            NodeIDs = new List<string>();

            title = groupTitle;
            OldTitle = groupTitle;

            SetPosition(new Rect(position, Vector2.zero));

            defaultBorderColor = contentContainer.style.borderBottomColor.value;
            defaultBorderWidth = contentContainer.style.borderBottomWidth.value;
        }

        public void SetErrorStyle(Color color)
        {
            contentContainer.style.borderBottomColor = color;
            contentContainer.style.borderBottomWidth = 2f;
        }

        public void ResetStyle()
        {
            contentContainer.style.borderBottomColor = defaultBorderColor;
            contentContainer.style.borderBottomWidth = defaultBorderWidth;
        }
    }
}