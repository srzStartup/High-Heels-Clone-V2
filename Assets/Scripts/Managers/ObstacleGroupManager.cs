using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ObstacleGroupManager : MonoBehaviour
{
    // Represents obstacles.
    private List<Transform> _childs;
    private bool _triggeredOnce = false;

    private void Start()
    {
        _childs = GetComponentsInChildren<Transform>()
            .ToList()
            .FindAll(child => !child.Equals(transform));
    }

    private bool UpperOneExists(Transform obstacle)
    {
        return _childs.Exists(child =>
        {
            return child.position.x == obstacle.position.x &&
                   child.position.z == obstacle.position.z &&
                   child.position.y > obstacle.position.y;
        });
    }

    private bool LowerOneExists(Transform obstacle)
    {
        return _childs.Exists(child =>
        {
            return child.position.x == obstacle.position.x &&
                   child.position.z == obstacle.position.z &&
                   child.position.y < obstacle.position.y;
        });
    }

    private int FindObstacleLevel(Transform obstacle)
    {
        return _childs.FindAll(child =>
        {
            return child.position.x == obstacle.position.x &&
                   child.position.z == obstacle.position.z &&
                   child.position.y < obstacle.position.y;
        }).Count + 1;
    }

    private Transform GetMainObstacle(Transform obstacle)
    {
        float greatestY = _childs.FindAll(child =>
        {
            return child.position.x == obstacle.position.x &&
                   child.position.z == obstacle.position.z &&
                   child.position.y > obstacle.position.y;
        })
            .ConvertAll(child => child.position.y)
            .Max();

        return _childs.Find(child => child.position.x == obstacle.position.x && child.position.z == obstacle.position.z && child.position.y == greatestY);
    }

    public int GetObstacleLevel(Transform obstacle)
    {
        if (_triggeredOnce) return 0;
        _triggeredOnce = true;
        Debug.Log(obstacle.name);
        if (UpperOneExists(obstacle))
        {
            Debug.Log("geçti");
            Transform obstacleTop = GetMainObstacle(obstacle);
            return FindObstacleLevel(obstacleTop);
        }
        if (LowerOneExists(obstacle))
        {
            Debug.Log("geçti");
            return FindObstacleLevel(obstacle);
        }
        Debug.Log("geçti");
        return 1;
    }

    public void AllTriggersOff()
    {
        _childs.ForEach(child => child.GetComponent<BoxCollider>().isTrigger = false);
    }
}
