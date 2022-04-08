using UnityEngine;
using System;

namespace BitByte{
public static class GetSelectedValue {
    public static System.Object Value(this SelectedValue selectedValue) {
        try {
            if (selectedValue.uniObject != null) return selectedValue.uniObject;
            if (selectedValue.sysObject != null) {
                if (selectedValue.valueType == typeof(Vector3)) return selectedValue.sysObject.ToVector3();
                if (selectedValue.valueType == typeof(Color) || selectedValue.valueType == typeof(Color32)) {
                    Color selectedColor;
                    ColorUtility.TryParseHtmlString(selectedValue.sysObject, out selectedColor);

                    if (selectedValue.valueType == typeof(Color32)) return (Color32)selectedColor;

                    return selectedColor;
                }
                if (selectedValue.valueType.IsEnum) {
                    return Enum.Parse(selectedValue.valueType, selectedValue.sysObject);
                }
                return Convert.ChangeType(selectedValue.sysObject, selectedValue.valueType);
            }
        } catch (System.Exception) {

            return null;
        }

        return null;
    }
}
}
