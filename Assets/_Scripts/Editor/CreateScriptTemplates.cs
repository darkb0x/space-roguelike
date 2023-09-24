using UnityEditor;

namespace Game
{
    public static class CreateScriptTemplates
    {
        private const string PATH = "Assets/Editor/ScriptTemplates/";
        private const string TEMPLATE_EXTENSION = ".cs.txt";
        private const string FILE_EXTENSION = ".cs";

        [MenuItem("Assets/Create/Script/MonoBehaviour", priority = 1)]
        public static void CreateMonoBehaviourMenuItem()
        {
            CreateScript("MonoBehaviour", "MonoBehaviour");
        }
        [MenuItem("Assets/Create/Script/ScriptableObject", priority = 2)]
        public static void CreateSOMenuItem()
        {
            CreateScript("ScriptableObject", "SO");
        }
        [MenuItem("Assets/Create/Script/C# Class", priority = 3)]
        public static void CreateCSharpClassItem()
        {
            CreateScript("CSharpClass", "Class");
        }
        [MenuItem("Assets/Create/Script/Interface", priority = 4)]
        public static void CreateInterfaceItem()
        {
            CreateScript("Interface", "IInterface");
        }
        [MenuItem("Assets/Create/Script/Enum", priority = 5)]
        public static void CreateEnumMenuItem()
        {
            CreateScript("Enum", "Enum");
        }

        private static void CreateScript(string template, string name)
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile($"{PATH}{template}{TEMPLATE_EXTENSION}", $"{name}{FILE_EXTENSION}");
        }
    }
}
