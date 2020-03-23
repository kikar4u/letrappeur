using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute
{
    public string conditionalSourceField = "";
    public bool hideInInspector = false;

    public ConditionalHideAttribute(string _conditionalSourceField)
    {
        this.conditionalSourceField = _conditionalSourceField;
        this.hideInInspector = false;
    }

    public ConditionalHideAttribute(string _conditionalSourceField, bool _hideInInspector)
    {
        this.conditionalSourceField = _conditionalSourceField;
        this.hideInInspector = _hideInInspector;
    }
}
