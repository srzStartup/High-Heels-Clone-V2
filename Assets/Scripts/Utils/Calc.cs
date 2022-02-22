using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Utility class to execute specific calculating operations.
public static class Calc
{
    // Calculates the median of the following axis from given bounds.
    public static float CalculateMidPoint(Bounds bounds, AxisType axis, float defaultValue = .0f)
    {
        FieldInfo fieldInfo = typeof(Vector3).GetField(axis.ToString());

        float? max = fieldInfo?.GetValue(bounds.max) as float?;
        float? min = fieldInfo?.GetValue(bounds.min) as float?;
        float? median = (max + min) / 2.0f;

        return median.GetValueOrDefault(defaultValue);
    }
}
