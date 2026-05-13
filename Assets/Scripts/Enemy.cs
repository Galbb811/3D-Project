using Unity.VisualScripting.FullSerializer;
using UnityEditor.Rendering;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //private CharacterController controller;
    public Transform player;
    //private Vector3 targetPoint;
    private Vector3 directionToPlayer;
    //public float rotationSpeed = 5;

    public float viewAngle = 120;
    public float viewRange = 5;
    public LayerMask playerLayer;
    public float detetctionRadius = 0.5f;
    //public float walkSpeed = 5;
    private NavMeshAgent agent;
    private bool patrolling = true;
    public Transform [] waypoints;
    private Transform targetWaypoint;
    private int waypointIndex = 0;
    private bool playerFound = false;
    public float alertDuration = 5;
    private float timeSinceAlerted = 0;
    private Vector3 tragetPosition;
    private Vector3 lastKnownPosition;
    private float timeWaited = 0;
    public float waitDuration = 2;
    private bool isWaiting = false;
    public float walkSpeed = 1;
    public float runSpeed = 3;
    


    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        SetNextTargetWaypoint(true);
    }

    private void SetNextTargetWaypoint(bool firstTime = false)
    { 
        if (!firstTime)
        {
         waypointIndex++;
        }
        
        if (waypointIndex >= waypoints.Length)
        {
            waypointIndex = 0;
            
        }
         targetWaypoint = waypoints [waypointIndex];

         agent.SetDestination(targetWaypoint.position);
    }

   
    void Update()
    {

        //targetPoint = new Vector3(player.position.x, transform.position.y, player.position.z);
        directionToPlayer = (player.position - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(directionToPlayer);
         if (patrolling)
        {
            Patrol();
        }

       if (PlayerDetected())
        {
            playerFound = true;
            patrolling = false;

            timeSinceAlerted = 0;
            timeWaited = 0;
            isWaiting = false;
            lastKnownPosition = player.position;
            agent.SetDestination(lastKnownPosition);
            //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, rot, agent.angularSpeed *Time.deltaTime);
        }
        //controller.Move(transform.forward * walkSpeed * Time.deltaTime);
       if (playerFound)
        {
            if (timeSinceAlerted < alertDuration)
            {
                //agent.SetDestination(player.position);
                timeSinceAlerted += Time.deltaTime;
            }
            else
            {
                playerFound = false;
                timeSinceAlerted = 0;
                patrolling = true;
                SetNextTargetWaypoint(true);
            }
        }
        
        agent.speed = playerFound? runSpeed : walkSpeed;
    }
    
    private void Patrol()
    {

        float dist = Vector3.Distance(transform.position, targetWaypoint.position);
        float buffer = 0.5f;
        //Debug.Log(dist);
        if (dist < buffer && !isWaiting)
        {
            //SetNextTargetWaypoint(true);
            isWaiting = true;
        } 
        if (isWaiting)
        {
            if (timeWaited < waitDuration)
            {
                Debug.Log("Waiting");
                timeWaited += Time.deltaTime;
            }
            else
            {
                SetNextTargetWaypoint();
                timeWaited = 0;
                isWaiting = false;
            }
        }
    }
    

    private bool PlayerDetected()
    {
        bool result = false;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        if (angle < viewAngle /2)
        {
            if (Physics.Raycast(transform.position, directionToPlayer, viewRange, playerLayer))
            {
                result = true;
            }
        }
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= detetctionRadius)
        {
            result = true;
        }


        return result;
        
    }
        
       
}
