﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
// ======================================================================================
//                                  SelectedValue
// ======================================================================================

namespace BitByte{
[CustomPropertyDrawer(typeof(SelectedValue))]
public class SelectedValueDrawer : PropertyDrawer {
    SerializedProperty valueType, uniObject, sysObject;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginProperty(position, label, property);

        Rect newPos = position;
        newPos.x = position.x + 5;
        newPos.height = singleLineHeight;

        DrawSelectedValue(newPos, property);

        EditorGUI.EndProperty();
        EditorGUI.EndChangeCheck();
        property.serializedObject.ApplyModifiedProperties();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUIUtility.singleLineHeight;
    }
    public float getHeight(SerializedProperty property, string name) {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(name));
    }
    public static void DrawSelectedValue(Rect position, SerializedProperty property) {
        SelectedValue selectedValue = (SelectedValue)property.GetTargetObjectOfProperty();
        if (selectedValue == null) return;
        System.Type selectedFieldType = selectedValue.valueType;
        if (selectedFieldType == null) return;
        if (typeof(UnityEngine.Object).IsAssignableFrom(selectedFieldType)) {
            property.SetTargetObjectOfProperty(propertyName: "sysObject", insideObject: true, value: null);

            if (selectedValue.uniObject != null)
                if (selectedValue.uniObject.GetType() != selectedFieldType)
                    property.SetTargetObjectOfProperty(propertyName: "uniObject", insideObject: true, value: null);

            selectedValue.uniObject = EditorGUI.ObjectField(position, "", selectedValue.uniObject, selectedFieldType, true);
            property.SetTargetObjectOfProperty(propertyName: "uniObject",
                                            insideObject: true,
                                            value: selectedValue.uniObject
                                            );

        } else {
            property.SetTargetObjectOfProperty(propertyName: "uniObject", insideObject: true, value: null);

            if (selectedFieldType.IsPrimitive || typeof(string).IsAssignableFrom(selectedFieldType)) {
                System.Object selValue = null;
                try {
                    selValue = Convert.ChangeType(selectedValue.sysObject, selectedFieldType);
                } catch (System.Exception) {
                    selValue = 0;
                }
                if (typeof(bool).IsAssignableFrom(selectedFieldType)) {
                    bool selectedBool = false;
                    try {
                        selectedBool = Convert.ToBoolean(selectedValue.sysObject);
                    } catch (System.Exception) {
                        selectedBool = false;
                    }
                    property.SetTargetObjectOfProperty(propertyName: "sysObject",
                                            insideObject: true,
                                            value: EditorGUI.Toggle(position, "", selectedBool).ToString()
                                            );
                } else {
                    property.SetTargetObjectOfProperty(propertyName: "sysObject",
                                            insideObject: true,
                                            value: EditorGUI.TextField(position, "", selValue.ToString())
                                            );
                }
            } else
            if (typeof(Vector3).IsAssignableFrom(selectedFieldType)) {
                Vector3 selectedVector3 = Vector3.zero;
                try {
                    selectedVector3 = selectedValue.sysObject.ToVector3();
                } catch (System.Exception) {
                    selectedVector3 = Vector3.zero;
                }
                property.SetTargetObjectOfProperty(propertyName: "sysObject",
                                            insideObject: true,
                                            value: EditorGUI.Vector3Field(position, "", selectedVector3).ConvertToString()
                                            );

            } else
            if (typeof(Vector2).IsAssignableFrom(selectedFieldType)) {
                Vector2 selectedVector2 = Vector2.zero;
                try {
                    selectedVector2 = selectedValue.sysObject.ToVector2();
                } catch (System.Exception) {
                    selectedVector2 = Vector2.zero;
                }
                property.SetTargetObjectOfProperty(propertyName: "sysObject",
                                            insideObject: true,
                                            value: EditorGUI.Vector2Field(position, "", selectedVector2).ConvertToString()
                                            );
            } else
            if (typeof(Color).IsAssignableFrom(selectedFieldType) || typeof(Color32).IsAssignableFrom(selectedFieldType)) {
                Color selectedColor = Color.black;
                try {
                    ColorUtility.TryParseHtmlString(selectedValue.sysObject, out selectedColor);
                } catch (System.Exception) {
                    selectedColor = Color.black;
                }
                selectedColor = EditorGUI.ColorField(position, selectedColor);

                property.SetTargetObjectOfProperty(propertyName: "sysObject",
                                           insideObject: true,
                                           value: "#" + ColorUtility.ToHtmlStringRGBA(selectedColor)
                                           );
            } else
            if (selectedFieldType.IsEnum) {
                int enumSelectedIndex = 0;
                string[] enumNames = Enum.GetNames(selectedFieldType);
                Array enumValues = Enum.GetValues(selectedFieldType);
                string[] enumWithValue = new string[enumNames.Length];
                for (int i = 0; i < enumNames.Length; i++) {
                    enumWithValue[i] = enumNames[i] + " - " + (int)enumValues.GetValue(i);
                }
                try {
                    // Index Of Selected Enum
                    enumSelectedIndex = enumNames.ToList().IndexOf(selectedValue.sysObject);
                    if (enumSelectedIndex > enumNames.Length || enumSelectedIndex < 0) enumSelectedIndex = 0;
                } catch (System.Exception) {
                    enumSelectedIndex = 0;
                }
                enumSelectedIndex = EditorGUI.Popup(position, enumSelectedIndex, enumWithValue);
                property.SetTargetObjectOfProperty(propertyName: "sysObject",
                                           insideObject: true,
                                           value: enumNames[enumSelectedIndex]
                                           );
            }
        }
    }
}
}
#endif