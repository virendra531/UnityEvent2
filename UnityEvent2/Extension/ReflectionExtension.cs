using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.Events;
using System;
using System.Runtime.CompilerServices;
using System.Linq;

namespace BitByte{
public static class ReflectionExtension {
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
    public static string getParametersString(this MethodInfo methodInfo) {
        string parameteString = "";
        ParameterInfo[] parameterInfo = methodInfo.GetParameters();
        for (int i = 0; i < parameterInfo.Length; i++) {
            if (parameterInfo.Length != 1)
                if (i != 0 && i != parameterInfo.Length) parameteString += ", ";

            string[] splitString = parameterInfo[i].ParameterType.ToString().Split('.');
            string lastString = splitString[splitString.Length - 1];

            parameteString += lastString;
        }
        return parameteString;
    }

    #region putSelectedValue
    public static void putSelectedValue(ref FieldInfo fieldInfo, UnityEngine.Object unityObj, string selectedField) {
        fieldInfo = null;
        if (selectedField == "") return;
        string[] splitString = selectedField.Split('/');
        if (splitString[1].Contains("(")) { fieldInfo = null; return; }

        fieldInfo = unityObj.GetType().GetField(splitString[1]);
        if (fieldInfo == null) { fieldInfo = null; return; }
        Type fieldType = fieldInfo.FieldType;
        if (typeof(UnityEngine.Events.UnityEvent).IsAssignableFrom(fieldType)) { fieldInfo = null; return; }
    }
    public static void putSelectedValue(ref PropertyInfo propertyInfo, UnityEngine.Object unityObj, string selectedField) {
        propertyInfo = null;
        if (selectedField == "") return;
        string[] splitString = selectedField.Split('/');
        if (splitString[1].Contains("(")) { propertyInfo = null; return; }

        propertyInfo = unityObj.GetType().GetProperty(splitString[1]);
    }
    public static void putSelectedValue(ref MethodInfo methodInfo, ref Type[] parameterTypes, ref Type returnType, ref Parameters[] parameters, UnityEngine.Object unityObj, string selectedField) {
        methodInfo = null;
        putSelectedValue(ref methodInfo, ref parameterTypes, ref returnType, unityObj, selectedField);
        //set function parameters
        if (methodInfo != null) {
            returnType = methodInfo.ReturnType;
            ParameterInfo[] methodParameters = methodInfo.GetParameters();
            if (methodParameters.Length != 0) {
                // set Parameter Array value
                ReflectionExtension.SetParametersValue(ref parameters, methodParameters);
            }
        }
    }
    public static void putSelectedValue(ref MethodInfo methodInfo, ref Type[] parameterTypes, ref Type returnType, UnityEngine.Object unityObj, string selectedField) {
        methodInfo = null;
        putSelectedValue(ref methodInfo, unityObj, selectedField);

        //set function parameters
        if (methodInfo != null) {
            returnType = methodInfo.ReturnType;
            ParameterInfo[] methodParameters = methodInfo.GetParameters();
            if (methodParameters.Length != 0) {
                List<Type> allTypes = new List<Type>();
                for (int y = 0; y < methodParameters.Length; y++) {
                    allTypes.Add(methodParameters[y].ParameterType);
                }
                parameterTypes = allTypes.ToArray();

                // set Parameter Array value
                // ReflectionExtension.SetParametersValue(ref parameters);
            }
        }
    }
    public static void putSelectedValue(ref MethodInfo methodInfo, UnityEngine.Object unityObj, string selectedField) {
        methodInfo = null;
        if (selectedField == "") return;
        string[] splitString = selectedField.Split('/');
        if (!splitString[1].Contains("(")) { methodInfo = null; return; }

        string functionName = splitString[1].Split('(')[0];

        methodInfo = unityObj.GetType().GetMethod(functionName);
    }
    public static void putSelectedValue(ref EventInfo eventInfo, UnityEngine.Object unityObj, string selectedField) {
        eventInfo = null;
        try {
            if (selectedField == "") return;
            string[] splitString = selectedField.Split('/');
            if (splitString[1].Contains("(")) { eventInfo = null; return; }

            eventInfo = unityObj.GetType().GetEvent(splitString[1]);
        } catch (System.Exception) {
            // throw;
        }

    }
    public static void putSelectedValue(ref UnityEvent unityEvent, UnityEngine.Object unityObj, string selectedField) {
        unityEvent = null;
        UnityEventBase unityEventBase = null;
        try {
            if (selectedField == "") return;
            string[] splitString = selectedField.Split('/');
            if (splitString[1].Contains("(")) { return; }

            FieldInfo fieldInfo;
            fieldInfo = unityObj.GetType().GetField(splitString[1]);
            if (fieldInfo != null) {
                Type fieldType = fieldInfo.FieldType;

                if (!typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(fieldType)) { return; }
                unityEventBase = (UnityEventBase)fieldInfo.GetValue(unityObj);

                unityEvent = (UnityEvent)fieldInfo.GetValue(unityObj);
            } else {
                PropertyInfo propertyInfo;
                propertyInfo = unityObj.GetType().GetProperty(splitString[1]);
                if (propertyInfo != null) {
                    Type propertyType = propertyInfo.PropertyType;

                    if (!typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(propertyType)) { return; }
                    unityEvent = (UnityEvent)propertyInfo.GetValue(unityObj, null);
                }
            }

        } catch (System.Exception ex) {
            Debug.Log(ex.Message);
            // throw;
        }

    }
    public static void putSelectedValue(ref UnityEvent unityEvent, ref System.Object obj, ref List<Type> types, UnityEngine.Object unityObj, string selectedField) {
        unityEvent = null;
        try {
            if (selectedField == "") return;
            string[] splitString = selectedField.Split('/');
            if (splitString[1].Contains("(")) { return; }

            FieldInfo fieldInfo;
            fieldInfo = unityObj.GetType().GetField(splitString[1]);
            if (fieldInfo != null) {
                Type fieldType = fieldInfo.FieldType;

                if (!typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(fieldType)) { return; }

                if (typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(fieldType)) {
                    ParameterInfo[] parameters = fieldType.GetMethod("Invoke").GetParameters();
                    for (int i = 0; i < parameters.Length; i++) {
                        types.Add(parameters[i].ParameterType);
                    }
                    obj = fieldInfo.GetValue(unityObj);
                }


                unityEvent = (UnityEvent)fieldInfo.GetValue(unityObj);
            } else {
                PropertyInfo propertyInfo;
                propertyInfo = unityObj.GetType().GetProperty(splitString[1]);
                if (propertyInfo != null) {
                    Type propertyType = propertyInfo.PropertyType;

                    if (!typeof(UnityEngine.Events.UnityEventBase).IsAssignableFrom(propertyType)) { return; }
                    unityEvent = (UnityEvent)propertyInfo.GetValue(unityObj, null);
                }
            }

        } catch (System.Exception ex) {
            Debug.Log(ex.Message);
            // throw;
        }

    }
    public static void putSelectedValue(ref SelectedValue selectedValue, FieldInfo fieldInfo, PropertyInfo propertyInfo) {
        if (fieldInfo != null) selectedValue.valueType = fieldInfo.FieldType;
        if (propertyInfo != null) selectedValue.valueType = propertyInfo.PropertyType;
    }
    #endregion
    public static System.Object GetValue(FieldInfo fieldInfo = null, PropertyInfo propertyInfo = null,
                                        MethodInfo methodInfo = null, Type[] parametersType = null, Parameters[] parameters = null,
                                        // Component comp = null, GameObject gm = null,
                                        UnityEngine.Object unityObj = null,
                                        string selectedField = "") {
        if (fieldInfo == null && propertyInfo == null && methodInfo == null) {
            string[] splitString = selectedField.Split('/');
            if (splitString[0] == splitString[1]) {
                return unityObj;
            }
        }

        System.Object returnValue = null;
        if (propertyInfo != null) {
            returnValue = propertyInfo.GetValue(unityObj, null);
        }
        if (fieldInfo != null) {
            returnValue = fieldInfo.GetValue(unityObj);
        }
        if (methodInfo != null) {
            if (methodInfo.GetParameters().Length == 0) {
                returnValue = methodInfo.Invoke(unityObj, null);
            } else {
                returnValue = methodInfo.Invoke(unityObj, GetValueFromParameters(parameters, parametersType));
            }
        }
        if (returnValue == null) return null;
        if (typeof(UnityEngine.Events.UnityEvent).IsAssignableFrom(returnValue.GetType()))
            return null;
        else
            return returnValue;

    }
    public static System.Object[] GetValueFromParameters(Parameters[] parameters, Type[] parametersType) {
        List<System.Object> parameterList = new List<System.Object>();
        for (int i = 0; i < parameters.Length; i++) {
            if (parameters[i].giveParameterAs == ValueOrGM.Value) {
                parameterList.Add(parameters[i].stringArg.Value());
            } else {
                if (parameters[i].unityObj != null) {
                    if (parameters[i].selectedField != null) {
                        parameterList.Add(parameters[i].selectedField.GetValue(parameters[i].unityObj));
                    }
                    if (parameters[i].selectedProperty != null) {
                        parameterList.Add(parameters[i].selectedProperty.GetValue(parameters[i].unityObj, null));
                    }
                    if (parameters[i].selectedMethodInfo != null) {
                        if (parameters[i].selectedMethodInfo.GetParameters().Length == 0) {
                            parameterList.Add(parameters[i].selectedMethodInfo.Invoke(parameters[i].unityObj, null));
                        } else {
                            List<System.Object> nestedParameterList = new List<System.Object>();
                            for (int y = 0; y < parameters[i].selectFunctionParameters.Length; y++) {
                                nestedParameterList.Add(parameters[i].selectFunctionParameters[y].Value());
                            }
                            parameterList.Add(parameters[i].selectedMethodInfo.Invoke(parameters[i].unityObj, nestedParameterList.ToArray()));
                        }
                    }
                }
            }
        }

        return parameterList.ToArray();
    }

    public static void SetValue(ref FieldInfo fieldInfo, ref PropertyInfo propertyInfo, System.Object firstValue, System.Object secondValue) {
        if (fieldInfo != null) {
            fieldInfo.SetValue(firstValue, secondValue);
        }
        if (propertyInfo != null) {
            propertyInfo.SetValue(firstValue, secondValue, null);
        }
    }
    public static void SetParametersValue(ref Parameters[] parameters, ParameterInfo[] methodParameters) {
        for (int x = 0; x < parameters.Length; x++) {
            if (parameters[x].unityObj == null || parameters[x].giveParameterAs == ValueOrGM.Value) {
                parameters[x].stringArg.valueType = methodParameters[x].ParameterType;
                parameters[x].returnType = methodParameters[x].ParameterType; continue;
            }
            ReflectionExtension.putSelectedValue(ref parameters[x].selectedField, parameters[x].unityObj, parameters[x].selectField);
            ReflectionExtension.putSelectedValue(ref parameters[x].selectedProperty, parameters[x].unityObj, parameters[x].selectField);

            ReflectionExtension.putSelectedValue(ref parameters[x].selectedMethodInfo,
                                                 ref parameters[x].methodParamTypes,
                                                 ref parameters[x].returnType,
                                                 parameters[x].unityObj, parameters[x].selectField);

            if (parameters[x].selectedMethodInfo != null) {
                parameters[x].returnType = parameters[x].selectedMethodInfo.ReturnType;
                ParameterInfo[] paramMethodParam = parameters[x].selectedMethodInfo.GetParameters();
                if (paramMethodParam.Length != 0) {
                    List<Type> paramMethodAllTypes = new List<Type>();
                    for (int y = 0; y < paramMethodParam.Length; y++) {
                        paramMethodAllTypes.Add(paramMethodParam[y].ParameterType);

                        parameters[x].selectFunctionParameters[x].valueType = paramMethodParam[y].ParameterType;
                    }
                    parameters[x].methodParamTypes = paramMethodAllTypes.ToArray();
                }
            }
        }
    }
    public static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, Type extType, string methodName) {
        IEnumerable<MethodInfo> query = from type in assembly.GetTypes()
                                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                                        from method in type.GetMethods(BindingFlags.Static
                                            | BindingFlags.Public | BindingFlags.NonPublic)
                                        where method.IsDefined(typeof(ExtensionAttribute), false)
                                        where method.GetParameters()[0].ParameterType == extType
                                        where method.Name == methodName
                                        select method;
        return query;
    }
}
}
