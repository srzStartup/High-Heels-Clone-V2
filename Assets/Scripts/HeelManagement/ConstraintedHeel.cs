using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstraintedHeel
{
    public Transform transform { get; private set; }

    public ParentConstraintStrategy parentStrategy { get; private set; }
    public PositionConstraintStrategy positionStrategy { get; private set; }

    public ConstraintedHeel(ConstraintedHeelBuilder heelBuilder)
    {
        transform = heelBuilder.transform;

        parentStrategy = heelBuilder.parentStrategy;
        positionStrategy = heelBuilder.positionStrategy;
    }
}
