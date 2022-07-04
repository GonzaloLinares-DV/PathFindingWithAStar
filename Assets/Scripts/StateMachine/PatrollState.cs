using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollState : MonoBehaviour
{
    public float speed;

    public List<Transform> patrolWaypoints = new List<Transform>();
    private int _currentWaypoint = 0;

    
    public void Patrol()
    {
        Vector3 dir = patrolWaypoints[_currentWaypoint].transform.position - transform.position;
        transform.forward = dir;
        transform.position += transform.forward * speed * Time.deltaTime;
        if (dir.magnitude < 0.15f)
        {
            _currentWaypoint++;
            if (_currentWaypoint > patrolWaypoints.Count - 1)
                _currentWaypoint = 0;
        }
    }

}
