using System;
using System.Collections.Generic;

using UnityEngine;

public class HeelsManagerBuilder : MonoBehaviour
{
    [SerializeField] private Heel left;
    [SerializeField] private Heel right;

    [SerializeField] private Transform _ground;

    [SerializeField] private float lengthPerSizing;
    [SerializeField, Range(.0f, 1.0f)] private float parentConstraintWeight = .25f;

    private List<ConstraintedHeel> heels;

    private const float defaultLengthPerSizing = .1f;

    public Vector3 heelExtents { get { return left.transform.GetComponent<Renderer>().bounds.extents; } }
    public Vector3 heelSize { get { return left.transform.GetComponent<Renderer>().bounds.size; } }

    private void Awake()
    {
        CreateHeelsManager(left, right);
    }

    private void CreateHeelsManager(params Heel[] heelTransforms)
    {
        heels = new List<ConstraintedHeel>();

        Array.ForEach(heelTransforms, (heel) =>
        {
            var constraintedHeel = ConstraintedHeelBuilder.Create(heel.transform)
                .SetParentSources(heel.parentSources)
                .SetPositionSources(heel.positionSources)
                .SetParentConstraintWeight(parentConstraintWeight)
                .Build();

            heels.Add(constraintedHeel);
        });

        var heelsManager = transform.gameObject.AddComponent<HeelsManager>();

        heelsManager.heels = heels;
        heelsManager.ground = _ground;
        heelsManager.lengthPerSizing = lengthPerSizing;

        Destroy(this);
    }
}
