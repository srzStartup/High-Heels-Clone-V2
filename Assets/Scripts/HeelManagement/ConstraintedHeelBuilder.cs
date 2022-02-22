using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstraintedHeelBuilder
{
    public Transform transform { get; private set; }
    public Transform[] parentSources { get; private set; }
    public Transform[] positionSources { get; private set; }

    public ParentConstraintStrategy parentStrategy { get; private set; }
    public PositionConstraintStrategy positionStrategy { get; private set; }

    public float parentConstraintWeight { get; private set; }

    private ConstraintedHeelBuilder(Transform heelTransform)
    {
        this.transform = heelTransform;
    }

    public static ConstraintedHeelBuilder Create(Transform heelTransform)
    {
        return new ConstraintedHeelBuilder(heelTransform);
    }

    public ConstraintedHeelBuilder SetParentSources(params Transform[] sources)
    {
        parentSources = sources;
        return this;
    }

    public ConstraintedHeelBuilder SetPositionSources(params Transform[] sources)
    {
        positionSources = sources;
        return this;
    }

    public ConstraintedHeelBuilder SetParentConstraintWeight(float weight)
    {
        parentConstraintWeight = weight;
        return this;
    }

    public ConstraintedHeel Build()
    {
        parentStrategy = new ParentConstraintStrategy(transform);
        positionStrategy = new PositionConstraintStrategy(transform);

        parentStrategy.AttachTo(parentConstraintWeight, parentSources);
        positionStrategy.AttachTo(sources: positionSources);

        return new ConstraintedHeel(this);
    }
}
