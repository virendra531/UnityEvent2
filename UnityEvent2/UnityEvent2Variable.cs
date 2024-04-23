using UnityEngine.Events;
using System.Reflection;
using System;

namespace BitByte {
#region All UnityAction Variable
//-------------------------Set Value | Call Function | Trigger Event----------------------------------
[System.Serializable]
public class UnityEvent2 {
    // public bool isVariableSet = false;
    public UnityEngine.Object unityObj;
    [FieldList("unityObj", addField: true, addProperty: true, onlyCanWrite: true, addEvent: true, addFunction: true, addValueReturnType: true, addVoidReturnType: true, disableName = true)]
    public string selectField;
    //----
    public ValueOrGM valueOrGM;
    //--
    public UnityEngine.Object unityObj2;
    [FieldList("unityObj2", addField: true, addProperty: true, addFunction: true, addValueReturnType: true, disableName = true)]
    public string selectField2;
    public Parameters[] parameters;
    //--
    [DrawValue("unityObj", "selectField")]
    public SelectedValue setValue;
    //---------------------------- For storage --------------------------
    //--PropertyField
    public FieldInfo selectedField;
    public PropertyInfo selectedProperty;
    //--Func
    public MethodInfo selectedMethodInfo;
    public Type[] methodParamTypes;
    public Type returnType;
    //--Event
    public EventInfo selectedEventInfo;
    public UnityEvent selectedUnityEvent;

    //---------------------------------------------------------------
    //------PropertyField
    public FieldInfo selectedField2;
    public PropertyInfo selectedProperty2;
}
#endregion
//----------------------------All Enum----------------------------------
public enum ValueOrGM {
    Value,
    Field
}
//---------------------------SelectedValue------------------------------------------
[System.Serializable]
public class SelectedValue {
    public Type valueType;
    public UnityEngine.Object uniObject;
    public string sysObject;
}
//----------------------------Function Parameters----------------------------------
[System.Serializable]
public class Parameters {
    public ValueOrGM giveParameterAs;

    public UnityEngine.Object unityObj;
    [FieldList(unityObj: "unityObj", addField: true, addProperty: true, addFunction: true, addValueReturnType: true, disableName = true)]
    public string selectField;

    [DrawValue(valueType: "methodParamTypes")]
    public SelectedValue[] selectFunctionParameters;

    [DrawValue(valueType: "returnType")]
    public SelectedValue stringArg;

    // For Storage
    public FieldInfo selectedField;
    public PropertyInfo selectedProperty;
    public MethodInfo selectedMethodInfo;
    public Type[] methodParamTypes;
    public Type returnType;
}
}
