using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

public class DetectPath3 : MonoBehaviour
{
    bool ispathnull;
    public PlayerNode playernode;
    public float speed;
    public List<Transform> patrolWaypoints = new List<Transform>();
    private int _currentWaypoint;
    public FOVAgent3 fovagent;
    public string TagPatroll;
    public bool SearchEnemy;
    private List<Node> AvailableTargets;
    private Node ClosestTarget;
    private Pathfinding _pf;
    public List<Node> path;
    public List<Node> path2;
    public Transform playerPos;
    public List<Node> patrolpath;
    
    List<Node> _detectedAgents = new List<Node>();

    public LayerMask detectableAgentMask;
    public LayerMask obstaclemask;
    public LayerMask obstacleMask;

    bool NodeDetected;
    public bool PatrolCollide;

    public float viewRadius;
    public float viewAngle;


    public float maxSpeed;
    public float maxForce;
    // Update is called once per frame
    private void Awake()
    {
        _pf = new Pathfinding();
        _pf.obstaclemask = obstaclemask;
        AvailableTargets = new List<Node>();

        PatrolCollide = false;
        SearchEnemy = false;
        ispathnull = true;
    }
    void Update()
    {
        if(fovagent.DetectedAgent==false)
        {
            FieldOfView();
            DetectTarget();

            if (PatrolCollide == true && SearchEnemy == false)
            {
                Patrol();
                ClearDetectableEnemies();
                if (ispathnull == false)
                {
                    path = null;
                    ispathnull = true;
                }


            }
            else if (PatrolCollide == false && SearchEnemy == false)
            {
                runPath();
                if(ispathnull == false)
                {
                    path = null;
                    ispathnull = true;
                }
            }
            else if(SearchEnemy)
            {
               
                PatrolCollide = false;
                runPath2();
                ispathnull = false;
            }
        }
        else
        {
            ClearDetectableEnemies();
            path = null;
            PatrolCollide = false;
        }
    }

   public void Xd()
    {
        PatrolCollide = false;
        SearchEnemy = false;
    }
   public void Patrol()
    {
        Vector3 dir = patrolWaypoints[_currentWaypoint].transform.position - transform.position;
        transform.parent.forward = dir;
        transform.parent.position += transform.forward * speed * Time.deltaTime;

        if (dir.magnitude < 0.15f)
        {
            _currentWaypoint++;
            if (_currentWaypoint > patrolWaypoints.Count - 1)
                _currentWaypoint = 0;
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
                if (InSight(transform.position,fovagent.playerFoundIn))
                {
                    _pf.ConstructPathAStar(ClosestTarget, playernode.ClosestTarget);
                }
                if (InSight(transform.position, item.transform.position))
                {
                    item.GetComponent<Node>().Detect(true);
                    _detectedAgents.Add(item.GetComponent<Node>());
                    AvailableTargets.Add(item.GetComponent<Node>());
                    Debug.DrawLine(transform.position, item.transform.position, Color.cyan);
                }
                else
                {
                    Debug.DrawLine(transform.position, item.transform.position, Color.red);
                }
            }
        }
    }
    public void runPath()
    {
        if (AvailableTargets.Count > 0 && PatrolCollide == false)
        {
            if (path == null || path.Count <= 0)
            {
                path = _pf.ConstructPathAStar(ClosestTarget, patrolpath[0]);

                path.Reverse();
            }
            else
            {

                Vector3 dir = path[0].transform.position - transform.parent.position;
                transform.parent.forward = dir;
                transform.parent.position += transform.parent.forward * 5 * Time.deltaTime;
                if (dir.magnitude < 0.1f) path.RemoveAt(0);
            }
        }
    }
    public void runPath2()
    {
        if (AvailableTargets.Count > 0)
        {
            if (path == null || path.Count <= 0)
            {
                
                path = _pf.ConstructPathAStar(ClosestTarget, playernode.ClosestTarget);
                path.Reverse();
            }
            else
            {

                Vector3 dir = path[0].transform.position - transform.parent.position;
                transform.parent.forward = dir;
                transform.parent.position += transform.parent.forward * 5 * Time.deltaTime;
                if (dir.magnitude < 0.1f) path.RemoveAt(0);

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

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag(TagPatroll) )
        {
            if (SearchEnemy == false && fovagent.DetectedAgent == false)
            {
                PatrolCollide = true;
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(TagPatroll))
        {
            if (SearchEnemy == false && fovagent.DetectedAgent == false)
            {
                PatrolCollide = true;
            }
        }
    }

}
