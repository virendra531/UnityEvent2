using UnityEngine;
using System;

namespace BitByte{
public static class StringVector {
    public static string ConvertToString(this Vector3 vector3) {
        string returnString = vector3.x + "," + vector3.y + "," + vector3.z;
        return returnString;
    }
    public static string ConvertToString(this Vector2 vector2) {
        string returnString = vector2.x + "," + vector2.y;
        return returnString;
    }
    public static Vector3 ToVector3(this string str) {
        string[] splitString = str.Split(',');
        Vector3 retrunVector = Vector3.zero;

        retrunVector.x = Convert.ToSingle(splitString[0]);
        retrunVector.y = Convert.ToSingle(splitString[1]);
        retrunVector.z = Convert.ToSingle(splitString[2]);

        return retrunVector;
    }
    public static Vector3 ToVector2(this string str) {
        string[] splitString = str.Split(',');
        Vector2 retrunVector = Vector2.zero;

        retrunVector.x = Convert.ToSingle(splitString[0]);
        retrunVector.y = Convert.ToSingle(splitString[1]);

        return retrunVector;
    }
}
}
