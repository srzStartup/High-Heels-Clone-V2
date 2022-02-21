using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenInputManager : Singleton<ScreenInputManager>
{
    public Vector3 lastHitPoint { get; private set; } = Vector3.zero;
    public float swipeDelta { get; private set; }

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.transform.position.z;

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                if (lastHitPoint != Vector3.zero)
                {
                    swipeDelta = hit.point.x - lastHitPoint.x;
                }
                else
                {
                    swipeDelta = 0;
                }

                lastHitPoint = hit.point;
            }
        }

        if (Input.GetButtonUp("Fire1"))
        {
            lastHitPoint = Vector3.zero;
            swipeDelta = 0;
        }
    }
}
