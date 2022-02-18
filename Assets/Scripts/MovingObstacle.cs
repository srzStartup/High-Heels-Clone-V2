using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] private float _travel;
    [SerializeField] private float _duration;

    private void Start()
    {
        transform.DOMoveX(transform.position.x + _travel, _duration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
