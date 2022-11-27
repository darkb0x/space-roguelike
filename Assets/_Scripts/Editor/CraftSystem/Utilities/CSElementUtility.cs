using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Game.CraftSystem.Editor.Utilities
{
    using Elements;

    public static class CSElementUtility
    {
        public static Button CreateButton(string text, Action onClick = null)
        {
            Button button = new Button(onClick)
            {
                text = text
            };

            return button;
        }

        public static Foldout CreateFoldout(string title, bool collapsed = false)
        {
            Foldout foldout = new Foldout()
            {
                text = title,
                value = !collapsed
            };

            return foldout;
        }

        public static Port CreatePort(this CSNode node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));

            port.portName = portName;

            return port;
        }

        public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label
            };

            if(onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }

            return textField;
        }

        public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(value, label, onValueChanged);
            textArea.multiline = true;

            return textArea;
        }

        public static ObjectField CreateObjectField(string name, Type valueType, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged)
        {
            ObjectField objectField = new ObjectField()
            {
                name = name,
                objectType = valueType
            };

            if(onValueChanged != null)
            {
                objectField.RegisterValueChangedCallback(onValueChanged);
            }

            return objectField;
        }

        public static IntegerField CreateIntField(string label, EventCallback<ChangeEvent<int>> onValueChanged)
        {
            IntegerField integerField = new IntegerField()
            {
                label = label
            };

            if(onValueChanged != null)
            {
                integerField.RegisterValueChangedCallback(onValueChanged);
            }

            return integerField;
        }
    }
}
