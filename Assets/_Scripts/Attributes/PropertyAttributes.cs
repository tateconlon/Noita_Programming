using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Note: doesn't work!
//From https://forum.unity.com/threads/favourite-way-to-serialize-interfaces.513874/#post-3364849
//https://github.com/lordofduct/spacepuppy-unity-framework-3.0/blob/2c4a60c84c5104d0f3e215f7dba7087a9e098b0e/SpacepuppyUnityFramework/PropertyAttributes.cs#L288
public abstract class SPPropertyAttribute : PropertyAttribute
    {

        public SPPropertyAttribute()
        {

        }

    }



    #region Component Attributes

    public abstract class ComponentHeaderAttribute : PropertyAttribute
    {

    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ForceRootTagAttribute : ComponentHeaderAttribute
    {

    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequireComponentInEntityAttribute : ComponentHeaderAttribute
    {

        private System.Type[] _types;

        public RequireComponentInEntityAttribute(params System.Type[] tps)
        {
            _types = tps;
        }

        public System.Type[] Types
        {
            get { return _types; }
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequireLayerAttribute : ComponentHeaderAttribute
    {
        public int Layer;

        public RequireLayerAttribute(int layer)
        {
            this.Layer = layer;
        }
    }
    
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UniqueToEntityAttribute : ComponentHeaderAttribute
    {

        public bool MustBeAttachedToRoot;
        public bool IgnoreInactive;

    }

    #endregion

    #region Property Drawer Attributes

    public class DisplayFlatAttribute : SPPropertyAttribute
    {

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class DisplayNestedPropertyAttribute : SPPropertyAttribute
    {

        public readonly string InnerPropName;
        public readonly string Label;
        public readonly string Tooltip;

        public DisplayNestedPropertyAttribute(string innerPropName)
        {
            InnerPropName = innerPropName;
        }

        public DisplayNestedPropertyAttribute(string innerPropName, string label)
        {
            InnerPropName = innerPropName;
            Label = label;
        }

        public DisplayNestedPropertyAttribute(string innerPropName, string label, string tooltip)
        {
            InnerPropName = innerPropName;
            Label = label;
            Tooltip = tooltip;
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class EnumFlagsAttribute : SPPropertyAttribute
    {

        public System.Type EnumType;

        public EnumFlagsAttribute()
        {

        }

        public EnumFlagsAttribute(System.Type enumType)
        {
            this.EnumType = enumType;
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class EnumPopupExcludingAttribute : SPPropertyAttribute
    {

        public readonly int[] excludedValues;

        public EnumPopupExcludingAttribute(params int[] excluded)
        {
            excludedValues = excluded;
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class EulerRotationInspectorAttribute : SPPropertyAttribute
    {

        public bool UseRadians = false;

        public EulerRotationInspectorAttribute()
        {

        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class GenericMaskAttribute : SPPropertyAttribute
    {

        private string[] _maskNames;

        public GenericMaskAttribute(params string[] names)
        {
            _maskNames = names;
        }

        public string[] MaskNames { get { return _maskNames; } }

    }

    /// <summary>
    /// Restrict a value to be no greater than max.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class MaxRangeAttribute : SPPropertyAttribute
    {
        public float Max;

        public MaxRangeAttribute(float max)
        {
            this.Max = max;
        }
    }

    /// <summary>
    /// Restrict a value to be no lesser than min.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class MinRangeAttribute : SPPropertyAttribute
    {
        public float Min;

        public MinRangeAttribute(float min)
        {
            this.Min = min;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class OneOrManyAttribute : SPPropertyAttribute
    {
        public OneOrManyAttribute()
        {

        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ReorderableArrayAttribute : SPPropertyAttribute
    {

        public string ElementLabelFormatString = null;
        public bool DisallowFoldout;
        public bool RemoveBackgroundWhenCollapsed;
        public bool Draggable = true;
        public float ElementPadding = 0f;
        public bool DrawElementAtBottom = false;
        public bool HideElementLabel = false;

        /// <summary>
        /// If DrawElementAtBottom is true, this child element can be displayed as the label in the reorderable list.
        /// </summary>
        public string ChildPropertyToDrawAsElementLabel;

        /// <summary>
        /// If DrawElementAtBottom is true, this child element can be displayed as the modifiable entry in the reorderable list.
        /// </summary>
        public string ChildPropertyToDrawAsElementEntry;

        public ReorderableArrayAttribute()
        {

        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class SelectableComponentAttribute : SPPropertyAttribute
    {
        public System.Type InheritsFromType;
        public bool AllowSceneObjects = true;
        public bool ForceOnlySelf = false;
        public bool SearchChildren = false;
        public bool AllowProxy;

        public SelectableComponentAttribute()
        {

        }

        public SelectableComponentAttribute(System.Type inheritsFromType)
        {
            this.InheritsFromType = inheritsFromType;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class SelectableObjectAttribute : SPPropertyAttribute
    {
        public System.Type InheritsFromType;
        public bool AllowSceneObjects = true;
        public bool AllowProxy;

        public SelectableObjectAttribute()
        {

        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class TagSelectorAttribute : SPPropertyAttribute
    {
        public bool AllowUntagged;
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class TimeUnitsSelectorAttribute : SPPropertyAttribute
    {
        public string DefaultUnits;

        public TimeUnitsSelectorAttribute()
        {
        }

        public TimeUnitsSelectorAttribute(string defaultUnits)
        {
            DefaultUnits = defaultUnits;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class TypeRestrictionAttribute : SPPropertyAttribute
    {
        public System.Type InheritsFromType;
        public bool HideTypeDropDown;
        public bool AllowProxy;

        public TypeRestrictionAttribute(System.Type inheritsFromType)
        {
            this.InheritsFromType = inheritsFromType;
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class UnitVectorAttribute : SPPropertyAttribute
    {

        public UnitVectorAttribute() : base()
        {

        }

    }

    /// <summary>
    /// A specialized PropertyDrawer that draws a struct/class in the shape:
    /// struct Pair
    /// {
    ///     float Weight;
    ///     UnityEngine.Object Value;
    /// }
    /// 
    /// It is drawn in the inspector as a single row as weight : value. 
    /// It is intended for use with arrays/lists of values that can be randomly selected by some weight.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class WeightedValueCollectionAttribute : ReorderableArrayAttribute
    {
        public string WeightPropertyName = "Weight";
        
        public WeightedValueCollectionAttribute(string weightPropName, string valuePropName)
        {
            this.WeightPropertyName = weightPropName;
            this.ChildPropertyToDrawAsElementEntry = valuePropName;
        }
    }

    #endregion

    #region Default Or Configured Property Drawer Attribute

    // public abstract class DefaultOrConfiguredAttribute : PropertyAttribute
    // {
    //
    //     private System.Type _fieldType;
    //     private object _defaultValue;
    //
    //     public DefaultOrConfiguredAttribute(System.Type tp)
    //     {
    //         _fieldType = tp;
    //         _defaultValue = tp.GetDefaultValue();
    //     }
    //
    //     public DefaultOrConfiguredAttribute(System.Type tp, object defaultValue)
    //     {
    //         _fieldType = tp;
    //         _defaultValue = defaultValue;
    //     }
    //
    //     public System.Type FieldType { get { return _fieldType; } }
    //
    //     public virtual bool DrawAsDefault(object value)
    //     {
    //         return object.Equals(value, _defaultValue);
    //     }
    //     public virtual object GetDefaultValue()
    //     {
    //         return _defaultValue;
    //     }
    //
    //     public virtual object GetValueToDisplayAsDefault()
    //     {
    //         return this.GetDefaultValue();
    //     }
    // }

    #endregion

    #region ModifierDrawer Attributes

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public abstract class PropertyModifierAttribute : SPPropertyAttribute
    {
        public bool IncludeChidrenOnDraw;
    }

    /// <summary>
    /// While in the editor, if the value is ever null, an attempt is made to get the value from self. You will still 
    /// have to initialize the value on Awake if null. The cost of doing it automatically is too high for all components 
    /// to test themselves for this attribute.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class DefaultFromSelfAttribute : PropertyModifierAttribute
    {
        public bool UseEntity = false;
        public bool HandleOnce = true;
    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class DisableOnPlayAttribute : PropertyModifierAttribute
    {

    }

/// <summary>
/// While in the editor, if the value is ever null, an attempt is made to find the value on a GameObject in itself 
/// that matches the name given.
/// 
/// You whil still have to initialize the value on Awake if null. The cost of doing it automatically is too high for all 
/// components to test themselves for this attribute.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
public class FindInSelfAttribute : PropertyModifierAttribute
{
    public string Name;
    public bool UseEntity = false;

    public FindInSelfAttribute(string name)
    {
        this.Name = name;
    }
    // }
    //
    // [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    // public class ForceFromSelfAttribute : PropertyModifierAttribute
    // {
    //
    //     public EntityRelativity Relativity;
    //
    //     public ForceFromSelfAttribute(EntityRelativity relativity)
    //     {
    //         this.Relativity = relativity;
    //     }
    // }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class OnChangedInEditorAttribute : PropertyModifierAttribute
    {

        public readonly string MethodName;
        public bool OnlyAtRuntime;

        public OnChangedInEditorAttribute(string methodName)
        {
            this.MethodName = methodName;
        }

    }

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ReadOnlyAttribute : PropertyModifierAttribute
    {

    }

    #endregion

    #region DecoratorDrawer Attributes

    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
    public class InsertButtonAttribute : PropertyAttribute
    {

        public string Label;
        public string OnClick;
        public bool PrecedeProperty;

        public InsertButtonAttribute(string label, string onClick)
        {
            this.Label = label;
            this.OnClick = onClick;
        }

    }
}
//
    // [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Class, AllowMultiple = false)]
    // public class InfoboxAttribute : ComponentHeaderAttribute
    // {
    //     public string Message;
    //     public InfoBoxMessageType MessageType = InfoBoxMessageType.Info;
    //
    //     public InfoboxAttribute(string msg)
    //     {
    //         this.Message = msg;
    //     }
    //
    // }
    //
    // #endregion
    //
    // #region NonSerialized Property Drawer Attributes
    //
    // public class ShowNonSerializedPropertyAttribute : System.Attribute
    // {
    //     public string Label;
    //     public string Tooltip;
    //     public bool Readonly;
    //
    //     public ShowNonSerializedPropertyAttribute(string label)
    //     {
    //         this.Label = label;
    //     }
    // }

    #endregion