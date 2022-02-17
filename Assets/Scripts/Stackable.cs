using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Stackable : MonoBehaviour
{
    [SerializeField] private Transform _node;
    [SerializeField] private Transform _multiParentedTransform;

    public void SetConstrainedObject(Transform tr)
    {
        tr.parent = _multiParentedTransform;
        tr.localPosition = Vector3.zero;

        MultiParentConstraint constraint = _multiParentedTransform.GetComponent<MultiParentConstraint>();

        WeightedTransform sourceObject = new WeightedTransform(_node, 1.0f);
        WeightedTransformArray sourceObjects = new WeightedTransformArray(1);

        sourceObjects.Add(sourceObject);

        constraint.data.sourceObjects = sourceObjects;
        constraint.data.constrainedObject = tr;
    }
}
