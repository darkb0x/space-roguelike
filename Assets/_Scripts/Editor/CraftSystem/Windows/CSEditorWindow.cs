using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;

namespace Game.CraftSystem.Editor.Windows
{
    using Utilities;

    public class CSEditorWindow : EditorWindow
    {
        private CSGraphView graphView;

        private readonly string defaultFileName = "TechTreeFileName";

        private static TextField fileNameTextField;
        private Button saveButton;
        private Button miniMapButton;

        [MenuItem("Window/Craft system/Craft Graph")]
        public static void Open()
        {
            GetWindow<CSEditorWindow>("Craft Graph");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolbar();

            AddStyles();
        }

        #region Elements addition
        private void AddGraphView()
        {
            graphView = new CSGraphView(this);

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }

        public void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();

            fileNameTextField = CSElementUtility.CreateTextField(defaultFileName, "File name:", callback =>
            {
                fileNameTextField.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            });

            saveButton = CSElementUtility.CreateButton("Save", () => Save());

            Button loadButton = CSElementUtility.CreateButton("Load", () => Load());
            Button clearButton = CSElementUtility.CreateButton("Clear", () => Clear());
            Button resetButton = CSElementUtility.CreateButton("Reset", () => ResetGraph());

            miniMapButton = CSElementUtility.CreateButton("Minimap", () => ToggleMiniMap());

            toolbar.Add(fileNameTextField);
            toolbar.Add(saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar.Add(miniMapButton);

            toolbar.AddStyleSheets("DialogueSystem/DSToolbarStyles.uss");

            rootVisualElement.Add(toolbar);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets(
                "DialogueSystem/DSVariables.uss"
            );
        }
        #endregion

        #region Toolbar actions
        private void Save()
        {
            if(string.IsNullOrEmpty(fileNameTextField.value))
            {
                EditorUtility.DisplayDialog(
                    "Invalid file name",
                    "Please ensure the file name you've typed in is valid.",
                    "Roger!"
                );

                return;
            }

            CSIOUtility.Initialize(graphView, fileNameTextField.value);
            CSIOUtility.Save();
        }

        private void Load()
        {
            string filePath = EditorUtility.OpenFilePanel("Craft Graphs", "Assets/Resources/CraftSystem/Graphs", "asset");

            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            Clear();

            CSIOUtility.Initialize(graphView, Path.GetFileNameWithoutExtension(filePath));
            CSIOUtility.Load();

            char[] trimEndElements = { 'G', 'r', 'a', 'p', 'h' };
            UpdateFileName(Path.GetFileNameWithoutExtension(filePath).TrimEnd(trimEndElements));
        }

        private void Clear()
        {
            graphView.ClearGraph();
        }

        private void ResetGraph()
        {
            Clear();

            UpdateFileName(defaultFileName);
        }

        private void ToggleMiniMap()
        {
            graphView.ToggleMiniMap();

            miniMapButton.ToggleInClassList("ds-toolbar__button__selected");
        }
        #endregion

        #region Utility methods
        public static void UpdateFileName(string newFileName)
        {
            fileNameTextField.value = newFileName;
        }

        public void EnableSaving()
        {
            saveButton.SetEnabled(true);
        }
        public void DisableSaving()
        {
            saveButton.SetEnabled(false);
        }
        #endregion
    }
}