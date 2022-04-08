using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BitByte{
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class FieldListAttribute : PropertyAttribute {
    public string unityObj;
    public bool ignoreGameObject = false,
                addField = false,
                addProperty = false,
                addEvent = false,
                addFunction = false,
                onlyCanWrite = false,
                onlyNumeric = false,
                addVoidReturnType = false,
                addValueReturnType = false,
                disableName = false;
    public FieldListAttribute(string unityObj,
                                bool addField = false,
                                bool addProperty = false,
                                bool addEvent = false,
                                bool addFunction = false,
                                bool onlyCanWrite = false,
                                bool onlyNumeric = false,
                                bool ignoreGameObject = false,
                                bool addVoidReturnType = false,
                                bool addValueReturnType = false,
                                bool disableName = false) {
        this.unityObj = unityObj;
        this.ignoreGameObject = ignoreGameObject;
        this.onlyCanWrite = onlyCanWrite;
        this.addField = addField;
        this.addProperty = addProperty;
        this.addFunction = addFunction;
        this.addEvent = addEvent;
        this.onlyNumeric = onlyNumeric;
        this.addVoidReturnType = addVoidReturnType;
        this.addValueReturnType = addValueReturnType;
        this.disableName = disableName;
    }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(FieldListAttribute))]
public class FieldListDrawer : PropertyDrawer {
    const BindingFlags flagsForProperty = // BindingFlags.NonPublic  
                                          //    |
                                        BindingFlags.Public
                                            |
                                        BindingFlags.Instance
                                            |
                                        BindingFlags.FlattenHierarchy
                                               |
                                            BindingFlags.Static
                                        //    |
                                        // BindingFlags.DeclaredOnly
                                        // |
                                        // BindingFlags.Default
                                        // |
                                        // BindingFlags.GetProperty
                                        // |
                                        // BindingFlags.GetField
                                        ;
    const BindingFlags flagsForField = // BindingFlags.NonPublic  
                                       //    |
                                        BindingFlags.Public
                                            |
                                        BindingFlags.Instance
                                               |
                                            BindingFlags.Static
                                                |
                                        BindingFlags.FlattenHierarchy
                                        //    |
                                        // BindingFlags.DeclaredOnly
                                        // |
                                        // BindingFlags.Default
                                        ;
    const BindingFlags flagsForFunc = // BindingFlags.NonPublic  
                                      //    |
                                        BindingFlags.Public
                                            |
                                        BindingFlags.Instance
                                               |
                                            BindingFlags.Static
                                                |
                                        BindingFlags.FlattenHierarchy
                                        //    |
                                        // BindingFlags.DeclaredOnly
                                        // |
                                        // BindingFlags.Default
                                        ;
    FieldListAttribute fieldListAttribClass { get { return (FieldListAttribute)attribute; } }
    int selectedIndex = 0;
    float propertyHeight = 0;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        int indent = EditorGUI.indentLevel;

        // Get Value from Attribute
        string unityObjString = fieldListAttribClass.unityObj;

        bool ignoreGameObject = fieldListAttribClass.ignoreGameObject;
        bool addField = fieldListAttribClass.addField;
        bool addProperty = fieldListAttribClass.addProperty;
        bool addFunction = fieldListAttribClass.addFunction;
        bool addEvent = fieldListAttribClass.addEvent;
        bool onlyCanWrite = fieldListAttribClass.onlyCanWrite;
        bool onlyNumeric = fieldListAttribClass.onlyNumeric;
        bool addValueReturnType = fieldListAttribClass.addValueReturnType;
        bool addVoidReturnType = fieldListAttribClass.addVoidReturnType;
        bool disableName = fieldListAttribClass.disableName;

        UnityEngine.Object unityObj = (UnityEngine.Object)property.GetTargetObjectOfProperty(unityObjString);
        if (unityObj == null) return;

        string[] allFields = GetFieldList(unityObj, addField, addProperty, addEvent,
                                    addFunction, onlyCanWrite, onlyNumeric, ignoreGameObject,
                                    addVoidReturnType, addValueReturnType);

        if (allFields == null || allFields.Length == 0) {
            propertyHeight = 0;
            if (allFields.Length == 0) propertyHeight = base.GetPropertyHeight(property, label);
            return;
        }

        // Currently Selected Name
        string currentFieldName = property.stringValue;

        // Index Of Selected Name
        selectedIndex = allFields.ToList().IndexOf(currentFieldName);
        if (selectedIndex > allFields.Length || selectedIndex < 0) selectedIndex = 0;

        // Create Popup list
        selectedIndex = EditorGUI.Popup(position, (disableName) ? "" : property.displayName, selectedIndex, allFields);

        // Change Selected Value
        property.stringValue = allFields[selectedIndex];


        // fill component variable
        if (unityObjString != null) {
            bool isGm = false;
            Component unityObjComp = null;
            GameObject unityObjGm = null;

            if (unityObj.GetType() == typeof(GameObject)) {
                isGm = true;
                unityObjGm = (GameObject)unityObj;
            } else {
                isGm = false;
                unityObjComp = (Component)unityObj;
            }

            string firstString = property.stringValue.Split('/')[0];

            if (isGm) {
                if (firstString == "GameObject") {
                    unityObj = unityObjGm;
                } else {
                    unityObj = unityObjGm.GetComponent(firstString) ?? unityObjGm.GetComponent(Type.GetType(firstString));
                }
            } else {
                if (firstString == "GameObject") {
                    unityObj = unityObjComp.gameObject;
                } else {
                    unityObj = unityObjComp.GetComponent(firstString) ?? unityObjComp.GetComponent(Type.GetType(firstString));
                }
            }

            property.SetTargetObjectOfProperty(unityObjString, value: unityObj);
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();

        propertyHeight = base.GetPropertyHeight(property, label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return propertyHeight;
    }
    public string[] GetFieldList(UnityEngine.Object unityObj, bool addField = false,
                                            bool addProperty = false,
                                            bool addEvent = false,
                                            bool addFunction = false,
                                            bool onlyCanWrite = false,
                                            bool onlyNumeric = false,
                                            bool ignoreGameObject = false,
                                            bool addVoidReturnType = false,
                                            bool addValueReturnType = false) {
        List<string> fieldNameList = new List<string>();

        Component[] allComponents;
        try {
            Component unityObjComp = (Component)unityObj;
            allComponents = unityObjComp.GetComponents<Component>();
        } catch (System.Exception) {
            GameObject unityObjGm = (GameObject)unityObj;
            allComponents = unityObjGm.GetComponents<Component>();
        }

        foreach (Component comp in allComponents) {
            string[] splitString = comp.GetType().ToString().Split('.');
            string lastString = splitString[splitString.Length - 1];

            if (addField) {
                GetAllFields(comp, ref fieldNameList, flagsForField, onlyNumeric);
            }
            if (addProperty) {
                fieldNameList.Add(lastString + "/" + lastString);
                GetAllProperty(comp, ref fieldNameList, flagsForProperty, onlyCanWrite, ignoreGameObject, onlyNumeric);
            }
            if (addEvent)
                GetAllEvent(comp, ref fieldNameList);
            if (addFunction)
                GetAllFunction(comp, ref fieldNameList, flagsForFunc, addVoidReturnType, addValueReturnType);
        }
        return fieldNameList.ToArray();
    }
    public void GetAllEvent(Component comp, ref List<string> fieldList) {
        string[] splitString = comp.GetType().ToString().Split('.');
        string lastString = splitString[splitString.Length - 1];

        EventInfo[] compEvents = comp.GetType().GetEvents();
        for (int i = 0; i < compEvents.Length; i++) {
            fieldList.Add(lastString + "/" + compEvents[i].Name);
        }
        FieldInfo[] compFields = comp.GetType().GetFields(flagsForField);
        for (int i = 0; i < compFields.Length; i++) {
            try {
                if (compFields[i].GetValue(comp) == null) continue;
            } catch (System.Exception) {
                continue;
                // throw;
            }
            Type fieldType = compFields[i].GetValue(comp).GetType();
            if (typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(fieldType)) {
                fieldList.Add(lastString + "/" + compFields[i].Name);
            }
        }
        PropertyInfo[] compProperty = comp.GetType().GetProperties(flagsForProperty);
        for (int i = 0; i < compProperty.Length; i++) {
            if (compProperty[i].IsDefined(typeof(ObsoleteAttribute), true)) continue;
            if (compProperty[i].Name == "Item" && comp.GetType() == typeof(Animation)) continue;
            if (compProperty[i].Name == "mesh" && comp.GetType() == typeof(MeshFilter)) continue;
            if (compProperty[i].Name == "material" || compProperty[i].Name == "materials") {
                if (typeof(Renderer).IsAssignableFrom(comp.GetType())) continue;
                if (typeof(Collider).IsAssignableFrom(comp.GetType())) continue;
            }
            try {
                if (compProperty[i].GetValue(comp, null) == null) continue;
            } catch (System.Exception) {
                continue;
            }

            Type propertyType = compProperty[i].GetValue(comp, null).GetType();
            if (typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(propertyType)) {
                fieldList.Add(lastString + "/" + compProperty[i].Name);
            }
        }
    }
    public void GetAllFunction(Component comp, ref List<string> fieldList, BindingFlags flags, bool addVoidReturnType = false, bool addValueReturnType = false) {
        string[] splitString = comp.GetType().ToString().Split('.');
        string lastString = splitString[splitString.Length - 1];

        if (comp.GetType() == typeof(Transform) || comp.GetType() == typeof(RectTransform)) {
            MethodInfo[] gmMethods = null;
            if (gmMethods == null)
                gmMethods = comp.gameObject.GetType().GetMethods(flagsForFunc);
            else
                gmMethods = gmMethods.Concat(comp.gameObject.GetType().GetMethods(flagsForFunc)).ToArray();

            for (int i = 0; i < gmMethods.Length; i++) {
                string funcName = gmMethods[i].Name;
                if (funcName.Contains("get_") || funcName.Contains("set_") || funcName.Contains("add_") || funcName.Contains("remove_")) continue;
                if (gmMethods[i].ReturnType == typeof(void)) {
                    if (addVoidReturnType)
                        fieldList.Add("GameObject" + "/" + funcName + "(" + gmMethods[i].getParametersString() + ")");
                } else {
                    if (addValueReturnType)
                        fieldList.Add("GameObject" + "/" + funcName + "(" + gmMethods[i].getParametersString() + ")");
                }
            }
        }

        MethodInfo[] compMethods = null;
        if (compMethods == null)
            compMethods = comp.GetType().GetMethods(flagsForFunc);
        else
            compMethods = compMethods.Concat(comp.GetType().GetMethods(flagsForFunc)).ToArray();

        for (int i = 0; i < compMethods.Length; i++) {
            string funcName = compMethods[i].Name;
            if (funcName.Contains("get_") || funcName.Contains("set_") || funcName.Contains("add_") || funcName.Contains("remove_")) continue;

            if (compMethods[i].ReturnType == typeof(void)) {
                if (addVoidReturnType)
                    fieldList.Add(lastString + "/" + funcName + "(" + compMethods[i].getParametersString() + ")");
            } else {
                if (addValueReturnType)
                    fieldList.Add(lastString + "/" + funcName + "(" + compMethods[i].getParametersString() + ")");
            }
        }
    }
    public void GetAllFields(Component comp, ref List<string> fieldList, BindingFlags flags, bool onlyNumeric) {
        string[] splitString = comp.GetType().ToString().Split('.');
        string lastString = splitString[splitString.Length - 1];

        FieldInfo[] compFields = comp.GetType().GetFields(flags);
        for (int i = 0; i < compFields.Length; i++) {
            try {
                if (compFields[i].GetValue(comp) == null) continue;
            } catch (System.Exception) {
                continue;
            }

            Type fieldType = compFields[i].GetValue(comp).GetType();

            if (onlyNumeric) {
                if (!IsNumericType(fieldType)) continue;
            }
            if (typeof(UnityEngine.Events.UnityEvent).IsAssignableFrom(fieldType)) continue;
            fieldList.Add(lastString + "/" + compFields[i].Name);
        }
    }
    public void GetAllProperty(Component comp, ref List<string> fieldList, BindingFlags flags, bool onlyCanWrite = false, bool ignoreGameObject = false, bool onlyNumeric = false) {
        PropertyInfo[] compProperty = comp.GetType().GetProperties(flagsForProperty);
        if (!ignoreGameObject && (comp.GetType() == typeof(Transform) || comp.GetType() == typeof(RectTransform))) {
            fieldList.Add("GameObject/GameObject");
            PropertyInfo[] gmProperty = comp.gameObject.GetType().GetProperties(flags);
            for (int i = 0; i < gmProperty.Length; i++) {
                if (onlyCanWrite)
                    if (!gmProperty[i].CanWrite) continue;

                if (gmProperty[i].IsDefined(typeof(ObsoleteAttribute), true)) continue;
                if (gmProperty[i].GetValue(comp.gameObject, null) == null) continue;

                Type propertyType = gmProperty[i].GetValue(comp.gameObject, null).GetType();
                if (onlyNumeric) {
                    if (!IsNumericType(propertyType)) continue;
                }
                if (typeof(UnityEngine.Events.UnityEvent).IsAssignableFrom(propertyType)) continue;
                fieldList.Add("GameObject" + "/" + gmProperty[i].Name);
            }
        }
        string[] splitString = comp.GetType().ToString().Split('.');
        string lastString = splitString[splitString.Length - 1];
        for (int i = 0; i < compProperty.Length; i++) {
            if (onlyCanWrite)
                if (!compProperty[i].CanWrite) continue;

            if (compProperty[i].IsDefined(typeof(ObsoleteAttribute), true)) continue;
            if (compProperty[i].Name == "Item" && comp.GetType() == typeof(Animation)) continue;
            if (compProperty[i].Name == "mesh" && comp.GetType() == typeof(MeshFilter)) continue;
            if (compProperty[i].Name == "material" || compProperty[i].Name == "materials") {
                fieldList.Add(lastString + "/" + compProperty[i].Name);

                if (typeof(Renderer).IsAssignableFrom(comp.GetType())) continue;
                if (typeof(Collider).IsAssignableFrom(comp.GetType())) continue;
            }

            if (compProperty[i].GetValue(comp, null) == null) continue;

            Type propertyType = compProperty[i].GetValue(comp, null).GetType();

            if (onlyNumeric) {
                if (!IsNumericType(propertyType)) continue;
            }
            if (typeof(UnityEngine.Events.UnityEvent).IsAssignableFrom(propertyType)) continue;

            fieldList.Add(lastString + "/" + compProperty[i].Name);
        }
    }
    public static bool IsNumericType(Type type) {
        if (type == null) {
            return false;
        }

        switch (Type.GetTypeCode(type)) {
            case TypeCode.Byte:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.SByte:
            case TypeCode.Single:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return true;
        }
        return false;
    }
    static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly,
        Type extendedType) {
        string[] splitString = extendedType.ToString().Split('.');

        var query = from type in assembly.GetTypes()
                    where type.IsSealed && !type.IsGenericType && !type.IsNested
                    from method in type.GetMethods(BindingFlags.Static
                        | BindingFlags.Public | BindingFlags.NonPublic)
                    where method.IsDefined(typeof(ExtensionAttribute), false)
                    where method.GetParameters()[0].ParameterType.ToString().Contains(splitString[splitString.Length - 1])
                    select method;
        return query;
    }
}
#endif
}