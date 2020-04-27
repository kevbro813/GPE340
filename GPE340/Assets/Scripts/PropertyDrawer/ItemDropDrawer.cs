using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ItemDrops))] // Create a custom property drawer for the ItemDrops class
public class ItemDropDrawer : PropertyDrawer // ItemDropDrawer derives from PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label) // Override the OnGUI method to reformat the inspector
    {
        EditorGUI.BeginProperty(pos, label, prop); // Begin the override logic for the property
        pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label); // Set the Prefix Label position
        int indent = EditorGUI.indentLevel; // Save the indent amount
        EditorGUI.indentLevel = 0; // Set the indent level to 0

        Rect objectRect = new Rect(pos.x, pos.y, pos.width - 40f, pos.height); // Set the position of the object property (drop item) in the inspector
        Rect chanceRect = new Rect(pos.x + pos.width - 40f, pos.y, 40f, pos.height); // Set the position of the chance property in the inspector

        // Draw the objectRect and chanceRect property fields
        EditorGUI.PropertyField(objectRect, prop.FindPropertyRelative("value"), GUIContent.none); // GUIContent.none will draw the GUI without labels
        EditorGUI.PropertyField(chanceRect, prop.FindPropertyRelative("chance"), GUIContent.none);

        EditorGUI.indentLevel = indent; // Reset indent to what it was before
        EditorGUI.EndProperty(); // End the override logic for the property
    }
}
#endif
