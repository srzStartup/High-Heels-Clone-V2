using UnityEngine;
using UnityEngine.Animations;

public class ParentConstraintStrategy : ConstraintStrategy<ParentConstraint>
{
    public ParentConstraintStrategy(Transform target, bool initialize = true)
        : base(target, initialize)
    {
    }

    public override void AttachTo(float weight = 1.0f, params Transform[] sources)
    {
        constraint.translationAtRest = Vector3.zero;
        constraint.rotationAtRest    = Vector3.zero;
        constraint.weight            = weight;

        constraint.SetSources(CreateConstraintSourceList(sources));

        ForEachSource((source) =>
        {
            Transform sourceTransform = source.sourceTransform;
            Vector3 positionOffset    = sourceTransform.InverseTransformPoint(target.position);
            Quaternion rotationOffset = Quaternion.Inverse(sourceTransform.rotation) * target.rotation;

            constraint.SetTranslationOffset(0, positionOffset);
            constraint.SetRotationOffset(0, rotationOffset.eulerAngles);
        });

        constraint.locked           = true;
        constraint.constraintActive = true;
    }
}
