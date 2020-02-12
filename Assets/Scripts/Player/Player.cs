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
    [Range(1, 100)]
    public float speed;
    public float gravity = 20.0f;
    [HideInInspector] public bool canMove, inCinematic, hasMovementControls;
    private PathCurve path;
    [SerializeField] private WaypointCurve nextWaypoint;
    [SerializeField] private WaypointCurve lastWaypoint;
    private int direction;

    [Tooltip("1 = Tourne instantanément ; 0 = Tourne pas")]
    [SerializeField] private float rotationSmoothness;

    [Tooltip("Angle entre le forward du personnage avec la position du prochain waypoint sous lequel le joueur pourra bouger")]
    [Range(0, 180)]
    [SerializeField] private float moveAfterRotationDegreeThreshold;
    private float forwardWayPointAngle;
    private Vector3 moveDirection;
    private Vector3 nextMoveDirection;
    [SerializeField] private float segmentBetweenWaypoint;
    [HideInInspector] public float movementOffset;

    LayerMask terrainMask;
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
        terrainMask = LayerMask.GetMask("Terrain");

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

        transform.position = lastWaypoint.waypointPosition.transform.position;

    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            raycastController.interactionAnim();
        }
        Rotate(nextMoveDirection);
        if (hasMovementControls && !inCinematic && Input.GetAxisRaw("Horizontal") != 0 && canMove)
        {
            WalkFollowingPath(speed);

            if (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")) != direction)
            {
                if (hasMovementControls)
                {
                    direction = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                    trapperAnim.SetAnimState(AnimState.WALK);

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
            trapperAnim.SetAnimState(AnimState.IDLE);
            Rotate(nextMoveDirection);

        }
        if (!characterController.isGrounded)
        {
            characterController.Move(new Vector3(0, -gravity * Time.deltaTime, 0));
        }

        forwardWayPointAngle = Vector3.Angle(new Vector3(nextMoveDirection.x - transform.position.x, 0f, nextMoveDirection.z - transform.position.z), new Vector3(transform.forward.x, 0f, transform.forward.z));

        if (Input.GetKeyDown("p"))
        {
            Respawn();
        }
    }

    public void WalkFollowingPath(float _speed)
    {
        //
        if (forwardWayPointAngle < moveAfterRotationDegreeThreshold)
        {

            if (Input.GetAxis("Horizontal") != 0f)
            {
                trapperAnim.SetAnimState(AnimState.WALK);
            }
            else
            {
                trapperAnim.SetAnimState(AnimState.IDLE);
            }

            segmentBetweenWaypoint = Mathf.Clamp01(segmentBetweenWaypoint);
            segmentBetweenWaypoint += Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * direction * _speed * 1 / Vector3.Distance(lastWaypoint.waypointPosition.transform.position, nextWaypoint.waypointPosition.transform.position);

            moveDirection = CalculateCurvePoint(segmentBetweenWaypoint);

            Rotate(moveDirection);
            movementOffset = (moveDirection - transform.position).magnitude;
            transform.position = new Vector3(moveDirection.x, transform.position.y, moveDirection.z);

            RaycastHit hit;
            //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + playerCollider.bounds.size.y, transform.position.z), transform.TransformDirection(Vector3.down) * playerCollider.bounds.size.y * 2, Color.yellow, 2f);
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + playerCollider.bounds.size.y, transform.position.z), transform.TransformDirection(Vector3.down) * playerCollider.bounds.size.y * 2, out hit, 3, terrainMask))
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hit.point.y, transform.position.z), 1);
            }

            if (segmentBetweenWaypoint > 1 || segmentBetweenWaypoint < 0)
            {
                ChangeWaypointTarget();
            }
        }

        nextMoveDirection = CalculateCurvePoint(segmentBetweenWaypoint + speed * Time.deltaTime * direction * 1 / Vector3.Distance(lastWaypoint.waypointPosition.transform.position, nextWaypoint.waypointPosition.transform.position));
    }

    Vector3 CalculateCurvePoint(float segment)
    {
        return Mathf.Pow(1 - segment, 3)
               * lastWaypoint.waypointPosition.transform.position +
                3 * Mathf.Pow(1 - segment, 2) * segment
                * lastWaypoint.bezierSecondPointPosition.transform.position +
                3 * (1 - segment) * Mathf.Pow(segment, 2)
                * nextWaypoint.bezierFirstPointPosition.position
                + Mathf.Pow(segment, 3) * nextWaypoint.waypointPosition.transform.position;
    }

    public void Rotate(Vector3 target)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,
        Quaternion.LookRotation(new Vector3(target.x - transform.position.x, 0f, target.z - transform.position.z)), rotationSmoothness);

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
        //Actualise le waypoint suivant et précédent
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

    public int GetDirection()
    {
        return direction;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Waypoint")
    //    {
    //        if (Input.GetAxisRaw("Horizontal") != 0)
    //        {
    //            //ChangeWaypointTarget();
    //        }
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.tag == "Waypoint")
    //    {
    //        if (other.gameObject == path.waypointCurves[0].waypointPosition && direction == -1 || other.gameObject == path.waypointCurves[path.waypointCurves.Length - 1].waypointPosition && direction == 1)
    //        {
    //            //canMove = false;
    //            //return;
    //        }

    //        else if (nextWaypoint.waypointPosition.GetComponent<Collider>() == other)
    //        {
    //            //SwapWaypointTarget(Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")));
    //        }
    //    }
    //}
}
