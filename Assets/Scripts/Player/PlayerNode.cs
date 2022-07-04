using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNode : MonoBehaviour
{
    public LayerMask obstacleMask;
    public LayerMask detectableAgentMask;
    private List<Node> AvailableTargets;
    public Node ClosestTarget;
    List<Node> _detectedAgents = new List<Node>();
    bool NodeDetected;
    public float viewRadius;
    public float viewAngle;

    private void Awake()
    {
        AvailableTargets = new List<Node>();
    }
    void Update()
    {
        FieldOfView();
        DetectTarget();
    }


    public void DetectTarget()
    {
        if (AvailableTargets.Count > 0)
        {
            ClosestTarget = AvailableTargets[0];
            float DistanceWithClosestTarget = Vector3.Distance(transform.position, AvailableTargets[0].transform.position);
            float DistanceFromNewTarget;
            for (int i = 0; i < AvailableTargets.Count; i++)
            {
                DistanceFromNewTarget = Vector3.Distance(transform.position, AvailableTargets[i].transform.position);
                if (DistanceFromNewTarget < DistanceWithClosestTarget)
                {
                    ClosestTarget = AvailableTargets[i];
                    DistanceWithClosestTarget = DistanceFromNewTarget;
                }
            }
            NodeDetected = true;
        }
        else
        {
            NodeDetected = false;
        }
    }
    void FieldOfView()
    {
        ClearDetectableEnemies();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, detectableAgentMask);

        foreach (var item in targetsInViewRadius)
        {
            Vector3 dirToTarget = (item.transform.position - transform.position);

            if (Vector3.Angle(transform.forward, dirToTarget.normalized) < viewAngle / 2)
            {
                if (InSight(transform.position, item.transform.position))
                {
                    item.GetComponent<Node>().Detect(true);
                    _detectedAgents.Add(item.GetComponent<Node>());
                    AvailableTargets.Add(item.GetComponent<Node>());
                    Debug.DrawLine(transform.position, item.transform.position, Color.yellow);
                    
                }
                else
                {
                    Debug.DrawLine(transform.position, item.transform.position, Color.red);
                }
            }
        }
    }
    void ClearDetectableEnemies()
    {
        foreach (var item in _detectedAgents)
        {
            item.Detect(false);
        }
        _detectedAgents.Clear();
        AvailableTargets.Clear();


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 lineA = DirFromAngle(viewAngle / 2 + transform.eulerAngles.y);
        Vector3 lineB = DirFromAngle(-viewAngle / 2 + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position, transform.position + lineA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + lineB * viewRadius);
    }




    bool InSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        return !Physics.Raycast(start, dir, dir.magnitude, obstacleMask);
    }


    Vector3 DirFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
