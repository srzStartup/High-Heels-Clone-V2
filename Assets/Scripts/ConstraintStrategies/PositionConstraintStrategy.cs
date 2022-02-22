using UnityEngine;
using UnityEngine.Animations;

public class PositionConstraintStrategy : ConstraintStrategy<PositionConstraint>
{
    public PositionConstraintStrategy(Transform target, bool initialize = true)
        : base(target, initialize)
    {
    }

    public override void AttachTo(float weight = 1.0f, params Transform[] sources)
    {
        constraint.translationAtRest = Vector3.zero;
        constraint.weight            = weight;

        constraint.SetSources(CreateConstraintSourceList(sources));

        constraint.locked           = true;
        constraint.constraintActive = true;
    }

    public void SetPivot(float x = 0, float y = 0, float z = 0)
    {
        constraint.locked = false;

        constraint.translationOffset = new Vector3(x, y, z);

        constraint.locked = true;
    }
}
