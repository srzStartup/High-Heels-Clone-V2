using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Animations;

public abstract class ConstraintStrategy<T> where T : Behaviour, IConstraint
{
    public Transform target { get; private set; }

    public T constraint
    {
        get { return target.GetComponent<T>(); }
    }

    public ConstraintStrategy(Transform target, bool initialize = true)
    {
        this.target = target;
        if (initialize) Init();
    }

    public abstract void AttachTo(float weight, params Transform[] sources);

    protected List<ConstraintSource> CreateConstraintSourceList(params Transform[] sources)
    {
        return sources.ToList()
            .ConvertAll((source) =>
            {
                return new ConstraintSource()
                {
                    sourceTransform = source,
                    weight = 1.0f
                };
            });
    }

    protected void ForEachSource(Action<ConstraintSource> action)
    {
        for (int i = 0; i < constraint.sourceCount; i++)
        {
            action(constraint.GetSource(i));
        }
    }

    public void Init()
    {
        if (constraint == null)
            target.gameObject.AddComponent<T>();
    }
}
