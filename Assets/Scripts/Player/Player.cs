using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Components
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Collider playerCollider;
    [HideInInspector] Animator animator;
    [HideInInspector] public TrapperAnim trapperAnim;
    [HideInInspector] public InteractionRaycast raycastController;
    #endregion

    #region Movements
    [Range(0, 1)]
    public float speed;
    public float gravity = 20.0f;
    [HideInInspector] public bool hasMovementControls;
    [HideInInspector] public bool canMove, inCinematic;
    private PathCurve path;
    [SerializeField] private WaypointCurve nextWaypoint;
    [SerializeField] private WaypointCurve lastWaypoint;
    private int direction;
    [Range(0, 0.1f)]
    [Tooltip("1 = Tourne instantanément ; 0 = Tourne pas")]
    [SerializeField] private float rotationSmoothness;
    [Tooltip("Angle entre le forward du personnage avec la position du prochain waypoint sous lequel le joueur pourra bouger")]
    [Range(0, 180)]
    [SerializeField] private float moveAfterRotationDegreeThreshold;
    private float forwardWayPointAngle;
    private Vector3 moveDirection;
    [SerializeField] private float segmentBetweenWaypoint;
    [HideInInspector] public float movementOffset;
    #endregion

    #region Checkpoint
    public Vector3 checkpointPosition;
    #endregion

    void Start()
    {
        #region Components
        playerCollider = GetComponent<Collider>();
        raycastController = GetComponent<InteractionRaycast>();
        trapperAnim = GetComponent<TrapperAnim>();
        path = GameObject.FindGameObjectWithTag("Path").GetComponent<PathCurve>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        #endregion

        #region MoveControl
        hasMovementControls = true;
        canMove = true;
        inCinematic = false;
        direction = 0;
        segmentBetweenWaypoint = 0;

        #endregion
        nextWaypoint = path.waypointCurves[1];
        lastWaypoint = path.waypointCurves[0];
        //transform.position = new Vector3(nextWaypoint.waypointPosition.transform.position.x, transform.position.y, nextWaypoint.waypointPosition.transform.position.z);
        transform.position = lastWaypoint.waypointPosition.transform.position;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            raycastController.interactionAnim();
        }
        if (hasMovementControls && !inCinematic && Input.GetAxisRaw("Horizontal") != 0)
        {
            WalkFollowingPath(speed);

            if (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")) != direction)
            {
                if (hasMovementControls)
                {
                    direction = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                    //direction = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                    //SwapWaypointTarget(Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")));

                }
                else
                {
                    SwapWaypointTarget(1);
                }
            }
        }
        else
        {
            movementOffset = 0;
        }
        if (!characterController.isGrounded)
        {
            characterController.Move(new Vector3(0, -gravity * Time.deltaTime, 0));
        }

        forwardWayPointAngle = Vector3.Angle(new Vector3(nextWaypoint.waypointPosition.transform.position.x - transform.position.x, 0f, nextWaypoint.waypointPosition.transform.position.z - transform.position.z), new Vector3(transform.forward.x, 0f, transform.forward.z));

        if (Input.GetKeyDown("p"))
        {
            Respawn();
        }

        //Rotate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Waypoint")
        {
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                //ChangeWaypointTarget();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Waypoint")
        {
            if (other.gameObject == path.waypointCurves[0].waypointPosition && direction == -1 || other.gameObject == path.waypointCurves[path.waypointCurves.Length - 1].waypointPosition && direction == 1)
            {
                //canMove = false;
                //return;
            }

            else if (nextWaypoint.waypointPosition.GetComponent<Collider>() == other)
            {
                //SwapWaypointTarget(Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")));
            }
        }
    }

    public void WalkFollowingPath(float _speed)
    {

        if (/*characterController.isGrounded &&*/ canMove)
        {
            //Si on veut bouger en fonction de sa rotation
            //Debug.Log(Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * transform.forward.z);

            //moveDirection = new Vector3(
            //    Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * transform.forward.x,
            //    0.0f,
            //    Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * transform.forward.z);

            segmentBetweenWaypoint = Mathf.Clamp01(segmentBetweenWaypoint);
            segmentBetweenWaypoint += Mathf.Abs(Input.GetAxis("Horizontal")) * direction * _speed * 1 / Vector3.Distance(lastWaypoint.waypointPosition.transform.position, nextWaypoint.waypointPosition.transform.position);

            moveDirection = Mathf.Pow(1 - segmentBetweenWaypoint, 3)
               * lastWaypoint.waypointPosition.transform.position +
                3 * Mathf.Pow(1 - segmentBetweenWaypoint, 2) * segmentBetweenWaypoint
                * lastWaypoint.bezierSecondPointPosition.transform.position +
                3 * (1 - segmentBetweenWaypoint) * Mathf.Pow(segmentBetweenWaypoint, 2)
                * nextWaypoint.bezierFirstPointPosition.position
                + Mathf.Pow(segmentBetweenWaypoint, 3) * nextWaypoint.waypointPosition.transform.position;

            //moveDirection -= transform.position;

            Rotate(moveDirection);
            movementOffset = (moveDirection - transform.position).magnitude;
            transform.position = new Vector3(moveDirection.x, transform.position.y, moveDirection.z);
            //Debug.Log("SegmentBetweenWaypoint :" + segmentBetweenWaypoint);

            if (segmentBetweenWaypoint > 1 || segmentBetweenWaypoint < 0)
            {
                ChangeWaypointTarget();
            }
        }
        //else if (!hasMovementControls)
        //{
        //    moveDirection = new Vector3(
        //        Time.deltaTime * transform.forward.x,
        //        0.0f,
        //        Time.deltaTime * transform.forward.z);
        //}

        //moveDirection *= _speed;
        //moveDirection.y -= gravity * Time.deltaTime;


        if (forwardWayPointAngle < moveAfterRotationDegreeThreshold && canMove)
        {
            //characterController.Move(new Vector3(0, -gravity * Time.deltaTime, 0));
            //characterController.Move(new Vector3(moveDirection.x, -gravity * Time.deltaTime, moveDirection.z));
            if (Input.GetAxis("Horizontal") != 0f)
            {
                trapperAnim.SetAnimState(AnimState.WALK);
            }
        }
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            trapperAnim.SetAnimState(AnimState.IDLE);
        }
    }

    public void Rotate(Vector3 target)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,
        Quaternion.LookRotation(new Vector3(target.x - transform.position.x, 0f, target.z - transform.position.z)),
        rotationSmoothness);
    }

    // direction > 0 = right
    void SwapWaypointTarget(int _direction)
    {
        WaypointCurve waypointSwitch;
        //Si on va à droite
        if (direction > 0)
        {
            //On verifie qu'on ne soit pas à une extrémité
            if (System.Array.IndexOf(path.waypointCurves, nextWaypoint) < path.waypointCurves.Length - 1)
            {
                if (!canMove)
                    canMove = true;
                //nextWaypoint = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, nextWaypoint) + 1];
                waypointSwitch = nextWaypoint;
                nextWaypoint = lastWaypoint;
                lastWaypoint = waypointSwitch;

                segmentBetweenWaypoint = 1 - segmentBetweenWaypoint;
                Debug.Log("Swap de points waypoint cible : " + System.Array.IndexOf(path.waypointCurves, nextWaypoint));
                Debug.Log("Swap de points last waypoint : " + System.Array.IndexOf(path.waypointCurves, lastWaypoint));
            }
            else
            {
                canMove = false;
            }
        }
        //Si on va à gauche
        else if (direction < 0)
        {
            if (System.Array.IndexOf(path.waypointCurves, nextWaypoint) > 0)
            {
                if (!canMove)
                    canMove = true;
                //nextWaypoint = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, nextWaypoint) - 1];
                waypointSwitch = nextWaypoint;
                nextWaypoint = lastWaypoint;
                lastWaypoint = waypointSwitch;
                segmentBetweenWaypoint = 1 - segmentBetweenWaypoint;
                Debug.Log("Swap de points waypoint cible : " + System.Array.IndexOf(path.waypointCurves, nextWaypoint));
                Debug.Log("Swap de points last waypoint : " + System.Array.IndexOf(path.waypointCurves, lastWaypoint));
            }
            else
            {
                Debug.Log("On est à une limite");
                canMove = false;
            }
        }
    }

    void ChangeWaypointTarget()
    {
        switch (direction)
        {
            case 1:
                if (System.Array.IndexOf(path.waypointCurves, nextWaypoint) < path.waypointCurves.Length - 1)
                {
                    lastWaypoint = nextWaypoint;
                    nextWaypoint = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, nextWaypoint) + 1];
                    segmentBetweenWaypoint = 0;
                }
                break;

            case -1:
                if (System.Array.IndexOf(path.waypointCurves, lastWaypoint) > 0)
                {
                    nextWaypoint = lastWaypoint;
                    lastWaypoint = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, nextWaypoint) - 1];
                    segmentBetweenWaypoint = 1;
                }
                break;
            default:
                break;
        }
    }

    public void Respawn()
    {
        transform.position = checkpointPosition;
    }
}
