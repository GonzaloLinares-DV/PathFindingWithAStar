using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FOVAgent3 : MonoBehaviour
{
    public List<FOVAgent3> FOVAgents = new List<FOVAgent3>();
    public List<DetectPath3> DetecterAgents = new List<DetectPath3>();
    public DetectPath3 detect;
    public bool DetectedAgent;
    public Vector3 PlayerPosSerch;
    public Vector3 playerFoundIn;
    public GameManager gameManager;
    public List<DetectableEnemy> _detectedAgents = new List<DetectableEnemy>();
    public LayerMask detectableAgentMask;
    public LayerMask obstacleMask;

    public GameObject target;

    private Vector3 _velocity;
    public float maxSpeed;
    public float maxForce;
    public float viewRadius;
    public float viewAngle;

    void Update()
    {
        playerFoundIn = gameManager.FoundIn;
       FieldOfView();

        if(_detectedAgents.Count>0)
        {
            Seek();
            detect.PatrolCollide = false;
            detect.path = null;
            for (int i = 0; i < DetecterAgents.Count; i++)
            {
                for (int o = 0; o < FOVAgents.Count; o++)
                {
                    if (FOVAgents[o]._detectedAgents.Count <= 0)
                    { DetecterAgents[i].SearchEnemy = true; }
                    else
                    { DetecterAgents[i].SearchEnemy = false; }

                }
                DetecterAgents[i].SearchEnemy = true; 

                DetecterAgents[i].PatrolCollide = false; 
            }
        }
        else
        {
            DetectedAgent = false;
            for (int i = 0; i < DetecterAgents.Count; i++)
            {
                DetecterAgents[i].SearchEnemy = false;
             
            }
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
                    if(item.GetComponent<DetectableEnemy>()!=null)
                    {
                        item.GetComponent<DetectableEnemy>().Detect(true);
                    }
                   
                    _detectedAgents.Add(item.GetComponent<DetectableEnemy>());
                    Debug.DrawLine(transform.position, item.transform.position, Color.black);

                    foreach (var VARIABLE in _detectedAgents)
                    {
                        PlayerPosSerch = gameManager.playerPos;
                        gameManager.FoundIn = PlayerPosSerch;
                    }
                    Seek();
                    transform.position += _velocity * Time.deltaTime;
                    transform.forward = _velocity.normalized;

                    
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
            if(item!=null)
                item.Detect(false);
        }
        _detectedAgents.Clear();
    }

    bool InSight(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        if (!Physics.Raycast(start, dir, dir.magnitude, obstacleMask)) return true;
        else return false;
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
    Vector3 DirFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    void Seek()
    {
        Vector3 desired = target.transform.position - transform.position;
        desired.Normalize();
        desired *= maxSpeed;

        Vector3 steering = desired - _velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);

        ApplyForce(steering);
    }

    void ApplyForce(Vector3 force)
    {
        _velocity += force;
    }
}
