using System;
using ParadoxNotion;
using ThirteenPixels.Soda;
using UnityEditor;
using UnityEngine;

// Reference: https://nodecanvas.paradoxnotion.com/documentation/?section=appending-custom-autoconverters
public static class SodaConverters
{
    [RuntimeInitializeOnLoadMethod]
#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#endif
    private static void Init()
    {
#pragma warning disable EventHandlerLeakAnalyzer
        TypeConverter.customConverter += OnConvert;
#pragma warning restore EventHandlerLeakAnalyzer
    }

    private static Func<object, object> OnConvert(Type sourceType, Type targetType)
    {
        if (sourceType == typeof(GlobalBool) && targetType == typeof(bool))
        {
            return value => value is GlobalBool globalBool ? globalBool.value : default;
        }
        
        if (sourceType == typeof(GlobalInt) && targetType == typeof(int))
        {
            return value => value is GlobalInt globalInt ? globalInt.value : default;
        }
        
        if (sourceType == typeof(GlobalFloat) && targetType == typeof(float))
        {
            return value => value is GlobalFloat globalFloat ? globalFloat.value : default;
        }
        
        if (sourceType == typeof(GlobalString) && targetType == typeof(string))
        {
            return value => value is GlobalString globalString ? globalString.value : default;
        }
        
        if (sourceType == typeof(GlobalVector2) && targetType == typeof(Vector2))
        {
            return value => value is GlobalVector2 globalVector2 ? globalVector2.value : default;
        }
        
        if (sourceType == typeof(GlobalVector3) && targetType == typeof(Vector3))
        {
            return value => value is GlobalVector3 globalVector3 ? globalVector3.value : default;
        }
        
        if (sourceType == typeof(GlobalGameObject) && targetType == typeof(GameObject))
        {
            return value => value is GlobalGameObject globalGameObject ? globalGameObject.value : default;
        }
        
        if (sourceType == typeof(GlobalGameObject) && targetType == typeof(Transform))
        {
            return value => value is GlobalGameObject globalGameObject ? globalGameObject.value.transform : default;
        }
        
        if (sourceType == typeof(GlobalGameObject) && targetType == typeof(Vector3))
        {
            return value => value is GlobalGameObject globalGameObject ? globalGameObject.value.transform.position : default;
        }
        
        return null;
    }
}