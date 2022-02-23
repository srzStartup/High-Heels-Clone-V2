using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Stackable : MonoBehaviour
{
    [SerializeField] private Transform _stackedObject;
    [SerializeField] private Transform _node;
    [SerializeField] private Transform _multiParentedTransform;

    public Stackable stackableObject { get; private set; }

    public Transform stackedObject => _stackedObject;

    public bool isReleased { get; set; }

    public void SetConstrainedObject(Stackable tr)
    {
        tr.transform.parent = _multiParentedTransform;
        tr.transform.localPosition = Vector3.zero;

        MultiParentConstraint constraint = _multiParentedTransform.GetComponent<MultiParentConstraint>();

        WeightedTransform sourceObject = new WeightedTransform(_node, 1);
        WeightedTransformArray sourceObjects = new WeightedTransformArray(1);

        sourceObjects.Add(sourceObject);

        constraint.data.sourceObjects = sourceObjects;
        constraint.data.constrainedObject = tr.transform;

        stackableObject = tr;
        stackableObject.isReleased = false;
    }

    public void ReleaseConstrainedObject()
    {
        MultiParentConstraint constraint = _multiParentedTransform.GetComponent<MultiParentConstraint>();
        Transform constrainedObject = constraint.data.constrainedObject;
        constrainedObject.parent = null;
        constraint.data.constrainedObject = null;
        constraint.data.sourceObjects.Clear();

        stackableObject.isReleased = true;
        stackableObject = null;
    }
}
