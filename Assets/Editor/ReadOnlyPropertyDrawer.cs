using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        UnityEngine.GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        UnityEngine.GUI.enabled = true;
    }
}
