using UnityEditor;
using TMPro.EditorUtilities;

namespace Game.Console
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Console_InputField), true)]
    public class Console_InputFieldEditor : TMP_InputFieldEditor { }
}