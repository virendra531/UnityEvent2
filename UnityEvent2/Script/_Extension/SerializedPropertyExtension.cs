#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using System.Collections.Generic;

namespace BitByte{
public static class SerializedPropertyExt {
    public static object GetTargetObjectOfProperty(this SerializedProperty prop, string propertyName = null, bool returnObject = false, bool insideObject = false) {
        string path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        List<string> elements = new List<string>(path.Split('.'));

        if (propertyName != null && insideObject == false) elements[elements.Count - 1] = propertyName;
        if (propertyName != null && insideObject == true) elements.Add(propertyName);

        if (returnObject && propertyName == null) {
            elements[elements.Count - 1] = null;
        }

        foreach (string element in elements) {
            if (element == null) continue;
            if (element.Contains("[")) {
                string elementName = element.Substring(0, element.IndexOf("["));
                int index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            } else {
                obj = GetValue_Imp(obj, element);
            }
        }
        return obj;
    }
    public static UnityEngine.Object GetTargetUnityObjectOfProperty(this SerializedProperty prop, string propertyName = null, bool returnObject = false, bool insideObject = false) {
        string path = prop.propertyPath.Replace(".Array.data[", "[");
        UnityEngine.Object obj = prop.serializedObject.targetObject;
        List<string> elements = new List<string>(path.Split('.'));

        if (propertyName != null && insideObject == false) elements[elements.Count - 1] = propertyName;
        if (propertyName != null && insideObject == true) elements.Add(propertyName);

        if (returnObject && propertyName == null) {
            elements[elements.Count - 1] = null;
        }

        foreach (string element in elements) {
            if (element == null) continue;
            if (element.Contains("[")) {
                string elementName = element.Substring(0, element.IndexOf("["));
                int index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = (UnityEngine.Object)GetValue_Imp(obj, elementName, index);
            } else {
                obj = (UnityEngine.Object)GetValue_Imp(obj, element);
            }
        }
        return obj;
    }
    public static void SetTargetObjectOfProperty(this SerializedProperty prop, string propertyName = null, bool returnObject = false, bool insideObject = false, System.Object value = null) {
        string path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        List<string> elements = new List<string>(path.Split('.'));

        if (propertyName != null && insideObject == false) elements[elements.Count - 1] = propertyName;
        if (propertyName != null && insideObject == true) elements.Add(propertyName);

        if (returnObject && propertyName == null) {
            elements[elements.Count - 1] = null;
        }
        for (int i = 0; i < elements.Count; i++) {
            if (elements[i] == null) continue;
            if (elements[i].Contains("[")) {
                string elementName = elements[i].Substring(0, elements[i].IndexOf("["));
                int index = System.Convert.ToInt32(elements[i].Substring(elements[i].IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            } else {
                if (obj == null)
                    return;
                var type = obj.GetType();
                while (type != null) {
                    var f = type.GetField(elements[elements.Count - 1], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (f != null) {
                        f.SetValue(obj, value);
                        break;
                    }
                    var p = type.GetProperty(elements[elements.Count - 1], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (p != null) {
                        p.SetValue(obj, value, null);
                        break;
                    }

                    type = type.BaseType;
                }
                obj = GetValue_Imp(obj, elements[i]);
            }
        }
    }


    private static void SetValue_Imp(object source, string name, System.Object value) {
        if (source == null)
            return;
        var type = source.GetType();

        while (type != null) {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                f.SetValue(source, value);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                p.SetValue(source, value, null);

            type = type.BaseType;
        }
    }


    private static object GetValue_Imp(object source, string name) {
        if (source == null)
            return null;
        var type = source.GetType();

        while (type != null) {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            type = type.BaseType;
        }
        return null;
    }
    private static object GetValue_Imp(object source, string name, int index) {
        var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
        if (enumerable == null) return null;
        var enm = enumerable.GetEnumerator();

        for (int i = 0; i <= index; i++) {
            if (!enm.MoveNext()) return null;
        }
        return enm.Current;
    }
}
}
#endif