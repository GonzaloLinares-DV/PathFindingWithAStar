using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntPlayer : MonoBehaviour
{
    public bool canRun;

    public DetectPath3 detect;
    public PlayerNode playernode;

    public FOVAgent3 fovagent;


    private List<Node> AvailableTargets;
    private Node ClosestTarget;
    private Pathfinding _pf;

    public List<Node> path;


    List<Node> _detectedAgents = new List<Node>();

    public LayerMask detectableAgentMask;
    public LayerMask obstaclemask;
    public LayerMask obstacleMask;

    bool NodeDetected;
   

    public float viewRadius;
    public float viewAngle;


    public float maxSpeed;
    public float maxForce;
    private void Awake()
    {
        _pf = new Pathfinding();
        _pf.obstaclemask = obstaclemask;
        AvailableTargets = new List<Node>();

        canRun = true;


    }
    void Update()
    {
        if (fovagent.DetectedAgent == false)
        {
            FieldOfView();
            DetectTarget();
            
        }
        else
        {
            ClearDetectableEnemies();
            path = null;
        }

       



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
            path = null;
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
                    Debug.DrawLine(transform.position, item.transform.position, Color.red);
                }
                else
                {
                    Debug.DrawLine(transform.position, item.transform.position, Color.green);
                }
            }
        }
    }




    public void runPath()
    {
       
            if (AvailableTargets.Count > 0)
            {
                if (path == null || path.Count <= 0)
                {
                    path = _pf.ConstructPathAStar(ClosestTarget, playernode.ClosestTarget);
                    path.Reverse();
                    return;
                }
                else
                {

                    Vector3 dir = path[0].transform.position - transform.parent.position;
                    transform.parent.forward = dir;
                    transform.parent.position += transform.parent.forward * 5 * Time.deltaTime;
                    if (dir.magnitude < 0.1f) path.RemoveAt(0);

                    float DistanceWithClosestTarget = Vector3.Distance(transform.position, playernode.ClosestTarget.transform.position);

                    if (1 > DistanceWithClosestTarget)
                    {
                        canRun = false;
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
        if (!Physics.Raycast(start, dir, dir.magnitude, obstacleMask)) return true;
        else return false;
    }


    Vector3 DirFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

   

}
