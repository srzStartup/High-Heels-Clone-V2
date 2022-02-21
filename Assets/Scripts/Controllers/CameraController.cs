using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Range(.0f, 100.0f)] private float distance;
    [SerializeField, Range(1.0f, 20.0f)] private float lerpTime;

    void Update()
    {
        Vector3 followFrom = new Vector3(target.position.x,
                                         transform.position.y,
                                         target.position.z - distance);

        transform.position = Vector3.Lerp(transform.position,
                                          followFrom,
                                          Time.deltaTime * lerpTime);
    }
}
