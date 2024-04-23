using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BitByte{
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class DrawValueAttribute : PropertyAttribute {
    public string unityObj, selectedField, sysObject, valueType;
    public DrawValueAttribute(string unityObj = null, string selectedField = null, string sysObject = null, string valueType = null) {
        this.unityObj = unityObj;
        this.selectedField = selectedField;
        this.sysObject = sysObject;
        this.valueType = valueType;
    }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(DrawValueAttribute))]
public class DrawValueDrawer : PropertyDrawer {
    DrawValueAttribute drawValueAttribute { get { return (DrawValueAttribute)attribute; } }

    float propertyHeight = 0;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginChangeCheck();
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        Type selectedFieldType = typeof(string);

        // Get Value from Attribute
        string unityObjString = drawValueAttribute.unityObj;
        string selectedField = drawValueAttribute.selectedField;
        string systemObject = drawValueAttribute.sysObject;
        string valueType = drawValueAttribute.valueType;

        if (valueType != null) {
            try {
                selectedFieldType = (Type)property.GetTargetObjectOfProperty("valueType", insideObject: true);
            } catch (Exception) {
                selectedFieldType = null;
            }
        } else {
            UnityEngine.Object unityObj = (UnityEngine.Object)property.GetTargetObjectOfProperty(unityObjString);
            if (unityObj == null) { property.SetTargetObjectOfProperty(value: null); return; }
            selectedField = (string)property.GetTargetObjectOfProperty(selectedField);

            FieldInfo fieldInfo = null;
            PropertyInfo propertyInfo = null;
            ReflectionExtension.putSelectedValue(ref fieldInfo, unityObj, selectedField);
            ReflectionExtension.putSelectedValue(ref propertyInfo, unityObj, selectedField);

            if (fieldInfo == null && propertyInfo == null) {
                string[] splitString = selectedField.Split('/');
                if (splitString.Length > 1) {
                    if (splitString[1].Contains('(')) return;
                    selectedFieldType = unityObj.GetType();
                }
            } else
                selectedFieldType = (fieldInfo != null) ? fieldInfo.FieldType : propertyInfo.PropertyType;
        }

        if (selectedFieldType == null) return;

        property.SetTargetObjectOfProperty("valueType", insideObject: true, value: selectedFieldType);

        DrawSelectedValue(position, property);

        EditorGUI.indentLevel = indent;


        propertyHeight = base.GetPropertyHeight(property, label);

        EditorGUI.EndProperty();
        EditorGUI.EndChangeCheck();
        property.serializedObject.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return propertyHeight;
    }
    public static void DrawSelectedValue(Rect position, SerializedProperty property) {
        SelectedValue selectedValue = (SelectedValue)property.GetTargetObjectOfProperty();
        Type selectedFieldType = selectedValue.valueType;

        if (typeof(UnityEngine.Object).IsAssignableFrom(selectedFieldType)) {
            property.SetTargetObjectOfProperty(propertyName: "sysObject", insideObject: true, value: null);

            if (selectedValue.uniObject != null)
                if (selectedValue.uniObject.GetType() != selectedFieldType)
                    property.SetTargetObjectOfProperty(propertyName: "uniObject", insideObject: true, value: null);

            property.SetTargetObjectOfProperty(propertyName: "uniObject",
                                            insideObject: true,
                                            value: EditorGUI.ObjectField(position, "", selectedValue.uniObject, selectedFieldType, true)
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
                    //throw;
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
#endif
}