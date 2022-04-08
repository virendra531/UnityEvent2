using System;
using System.Reflection;

namespace BitByte{
public static class UnityEvent2Extension {
    public static void SetVariable(this UnityEvent2[] unityEvent2) {
        if (unityEvent2 == null) return;
        for (int z = 0; z < unityEvent2.Length; z++) {
            unityEvent2[z].SetVariable();
        }
    }
    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void SetVariable(this UnityEvent2 unityEvent2) {
        if (unityEvent2 == null) return;
        if (unityEvent2.unityObj == null) return;
        //------------------------------For First Variable-------------------------
        if (unityEvent2.selectField.Contains("(")) {
            ReflectionExtension.putSelectedValue(ref unityEvent2.selectedMethodInfo,
                                                ref unityEvent2.methodParamTypes,
                                                ref unityEvent2.returnType,
                                                ref unityEvent2.parameters,
                                                unityEvent2.unityObj, unityEvent2.selectField);
        } else {
            //------------------------------For First Variable-------------------------
            ReflectionExtension.putSelectedValue(ref unityEvent2.selectedField, unityEvent2.unityObj, unityEvent2.selectField);
            ReflectionExtension.putSelectedValue(ref unityEvent2.selectedProperty, unityEvent2.unityObj, unityEvent2.selectField);
            ReflectionExtension.putSelectedValue(ref unityEvent2.selectedEventInfo, unityEvent2.unityObj, unityEvent2.selectField);
            ReflectionExtension.putSelectedValue(ref unityEvent2.selectedUnityEvent, unityEvent2.unityObj, unityEvent2.selectField);
            //------------------------------For Second Variable-------------------------
            if (unityEvent2.valueOrGM == ValueOrGM.Value) {
                ReflectionExtension.putSelectedValue(ref unityEvent2.setValue, unityEvent2.selectedField, unityEvent2.selectedProperty);
            } else {
                ReflectionExtension.putSelectedValue(ref unityEvent2.selectedField2, unityEvent2.unityObj2, unityEvent2.selectField2);
                ReflectionExtension.putSelectedValue(ref unityEvent2.selectedProperty2, unityEvent2.unityObj2, unityEvent2.selectField2);
                ReflectionExtension.putSelectedValue(ref unityEvent2.selectedMethodInfo,
                                                ref unityEvent2.methodParamTypes,
                                                ref unityEvent2.returnType,
                                                ref unityEvent2.parameters,
                                                unityEvent2.unityObj2, unityEvent2.selectField2);
            }
        }
    }
    public static void Invoke(this UnityEvent2[] unityEvent2) {
        if (unityEvent2.Length == 0) return;
        for (int i = 0; i < unityEvent2.Length; i++) {
            unityEvent2[i].Invoke();
        }
    }
    public static void InvokeMultipleIndex(this UnityEvent2[] unityEvent2, string indexString) {
        if (indexString == "") return;
        if (unityEvent2.Length == 0) return;
        string[] allIndex = indexString.Split(',');
        for (int y = 0; y < allIndex.Length; y++) {
            int index = Int32.Parse(allIndex[y]);
            if (index < 0 || index > unityEvent2.Length) continue;

            unityEvent2[index].Invoke();
        }
    }
    public static void Invoke(this UnityEvent2 unityEvent2) {
        if (unityEvent2.unityObj != null) {
            if (unityEvent2.selectedField == null && unityEvent2.selectedProperty == null && unityEvent2.selectedEventInfo == null) {
                unityEvent2.SetVariable();
            }
            if (unityEvent2.selectField.Contains("(")) {
                System.Object returnValue = CallFunction(unityEvent2);
            } else {
                TriggerEvent(unityEvent2);
                SetValue(unityEvent2);
            }
        }
    }
    public static System.Object InvokeWithReturn(this UnityEvent2 unityEvent2) {
        if (unityEvent2.unityObj != null) {
            if (unityEvent2.selectField == null && unityEvent2.selectedProperty == null && unityEvent2.selectedEventInfo == null) {
                unityEvent2.SetVariable();
            }
            if (unityEvent2.selectField.Contains("(")) {
                return CallFunction(unityEvent2);
            }
        }
        return null;
    }
    private static System.Object CallFunction(UnityEvent2 unityEvent2) {
        return ReflectionExtension.GetValue(methodInfo: unityEvent2.selectedMethodInfo,
                                                parametersType: unityEvent2.methodParamTypes,
                                                parameters: unityEvent2.parameters,
                                                unityObj: unityEvent2.unityObj,
                                                selectedField: unityEvent2.selectField);
    }

    private static void TriggerEvent(UnityEvent2 unityEvent2) {
        if (unityEvent2.selectedUnityEvent != null)
            unityEvent2.selectedUnityEvent.Invoke();
        if (unityEvent2.selectedEventInfo != null) {
            MethodInfo eventRaiseMethod = unityEvent2.selectedEventInfo.GetRaiseMethod();
            eventRaiseMethod.Invoke(unityEvent2.unityObj, null);
        }
    }
    private static void SetValue(UnityEvent2 unityEvent2) {
        Type firstValueType = null;
        System.Object secondValue = null;

        //Get FirstValue Type
        if (unityEvent2.selectedField != null) firstValueType = unityEvent2.selectedField.FieldType;
        if (unityEvent2.selectedProperty != null) firstValueType = unityEvent2.selectedProperty.PropertyType;

        if (firstValueType == null) return;

        // Get Second Value
        if (unityEvent2.valueOrGM == ValueOrGM.Value) {
            secondValue = unityEvent2.setValue.Value();
        } else {
            secondValue = ReflectionExtension.GetValue(fieldInfo: unityEvent2.selectedField2,
                                                propertyInfo: unityEvent2.selectedProperty2,
                                                methodInfo: unityEvent2.selectedMethodInfo,
                                                parametersType: unityEvent2.methodParamTypes,
                                                parameters: unityEvent2.parameters,
                                                unityObj: unityEvent2.unityObj2,
                                                selectedField: unityEvent2.selectField2);
        }
        // Now Set value
        ReflectionExtension.SetValue(ref unityEvent2.selectedField,
                            ref unityEvent2.selectedProperty, unityEvent2.unityObj, secondValue);
    }
}
}