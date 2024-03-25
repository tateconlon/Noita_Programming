using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Now you can use interfaces in the inspector!
//https://bitbucket.org/gaello/interface-in-inspector/src/master/
//https://www.patrykgalach.com/2020/01/27/assigning-interface-in-unity-inspector/

[CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
public class RequireInterfaceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            RequireInterfaceAttribute requiredAtrribute = 
                this.attribute as RequireInterfaceAttribute;

            EditorGUI.BeginProperty(position, label, property);
            
            property.objectReferenceValue = 
                EditorGUI.ObjectField(position, label, property.objectReferenceValue, 
                    requiredAtrribute.requiredType, true);
            
            EditorGUI.EndProperty();
        }
        else
        {
            Color prevColor = GUI.color;
            GUI.color = Color.red;
            
            EditorGUI.LabelField(position, label, new GUIContent("Property is not a reference type"));

            GUI.color = prevColor;
        }
    }
}
