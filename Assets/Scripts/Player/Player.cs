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
    [HideInInspector] public bool canMove, inCinematic, hasMovementControls, blocked;
    private PathCurve path;
    private CurvedPositionInfo currentCurvedPosInfo;
    private CurvedPositionInfo respawnCurvedPosInfo;
    private int direction;

    [Tooltip("1 = Tourne instantanément ; 0 = Tourne pas")]
    [SerializeField] private float rotationSmoothness;

    [Tooltip("Angle entre le forward du personnage avec la position du prochain waypoint sous lequel le joueur pourra bouger")]
    [Range(0, 180)]
    [SerializeField] private float moveAfterRotationDegreeThreshold;
    private float forwardWayPointAngle;
    private Vector3 moveDirection;
    [HideInInspector] public Vector3 nextMoveDirection;
    [HideInInspector] public float movementOffset;

    LayerMask terrainMask;
    #endregion

    #region Checkpoint
    Vector3 lastCheckpointPosition;
    #endregion

    void Start()
    {
        currentCurvedPosInfo = new CurvedPositionInfo();
        respawnCurvedPosInfo = new CurvedPositionInfo();

        #region Components
        playerCollider = GetComponent<Collider>();
        raycastController = GetComponent<InteractionRaycast>();
        trapperAnim = GetComponent<TrapperAnim>();
        path = GameObject.FindGameObjectWithTag("Path").GetComponent<PathCurve>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        terrainMask = LayerMask.GetMask("Ground");
        #endregion

        #region MoveControl
        hasMovementControls = true;
        canMove = true;
        inCinematic = false;
        direction = 0;
        currentCurvedPosInfo.segmentBetweenWaypoint = 0;

        #endregion
        currentCurvedPosInfo.nextWaypoint = path.waypointCurves[1];
        currentCurvedPosInfo.lastWaypoint = path.waypointCurves[0];
        currentCurvedPosInfo.segmentBetweenWaypoint = 0;
        SaveCurrentPosInfo();
        transform.position = currentCurvedPosInfo.lastWaypoint.waypointPosition.transform.position;
        Fader.Instance.respawnDelegate += Respawn;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            raycastController.interactionAnim();
        }
        Rotate(nextMoveDirection);
        if (hasMovementControls && !inCinematic && Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")) != 0 && canMove)
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
        else if (trapperAnim.GetCurrentState() != AnimState.CLIMB && trapperAnim.GetCurrentState() != AnimState.BREATH)
        {
            movementOffset = 0;
            trapperAnim.SetAnimState(AnimState.IDLE);
            //Rotate(nextMoveDirection);
        }

        if (!characterController.isGrounded)
        {
            characterController.Move(new Vector3(0, -gravity * Time.deltaTime, 0));
        }

        forwardWayPointAngle = Vector3.Angle(new Vector3(nextMoveDirection.x - transform.position.x, 0f, nextMoveDirection.z - transform.position.z), new Vector3(transform.forward.x, 0f, transform.forward.z));

    }
    public void PlayFootstep()
    {
        RaycastHit hit;
        // faire une détection de si c'est de la pierre ou pas
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + playerCollider.bounds.size.y, transform.position.z), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, terrainMask) && hit.transform.tag == "rock")
        {
            GameObject.FindGameObjectWithTag("SoundManager").GetComponent<_MGR_SoundDesign>().
       PlaySound("FootStepRock", GetComponent<AudioSource>());

        }
        else
        {
            GameObject.FindGameObjectWithTag("SoundManager").GetComponent<_MGR_SoundDesign>().
                PlaySound("FootStepSnow", GetComponent<AudioSource>());
        }

    }
    public void WalkFollowingPath(float _speed)
    {
        if (forwardWayPointAngle < moveAfterRotationDegreeThreshold && !blocked)
        {
            //if (Input.GetAxis("Horizontal") != 0f && trapperAnim.GetCurrentState() != AnimState.CLIMB)
            //{
            //    trapperAnim.SetAnimState(AnimState.WALK);
            //}

            currentCurvedPosInfo.segmentBetweenWaypoint = Mathf.Clamp01(currentCurvedPosInfo.segmentBetweenWaypoint);

            if (Input.GetAxisRaw("Horizontal") != 0f && (trapperAnim.GetCurrentState() == AnimState.WALK || trapperAnim.GetCurrentState() == AnimState.IDLE))
            {
                currentCurvedPosInfo.segmentBetweenWaypoint += Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * direction * _speed * 1 / Vector3.Distance(currentCurvedPosInfo.lastWaypoint.waypointPosition.transform.position, currentCurvedPosInfo.nextWaypoint.waypointPosition.transform.position);

                if (trapperAnim.GetCurrentState() != AnimState.WALK)
                {
                    trapperAnim.SetAnimState(AnimState.WALK);
                }
            }
            else if (trapperAnim.GetCurrentState() == AnimState.CLIMB || trapperAnim.GetCurrentState() == AnimState.PASSIVE_WALK)
            {
                currentCurvedPosInfo.segmentBetweenWaypoint += Time.deltaTime * direction * _speed * 1 / Vector3.Distance(currentCurvedPosInfo.lastWaypoint.waypointPosition.transform.position, currentCurvedPosInfo.nextWaypoint.waypointPosition.transform.position);
                Debug.Log("Move by breathing or climb : " + _speed);
            }

            moveDirection = currentCurvedPosInfo.CalculateCurvePoint(currentCurvedPosInfo.segmentBetweenWaypoint);
            moveDirection = new Vector3(moveDirection.x, transform.position.y, moveDirection.z);

            Rotate(moveDirection);
            movementOffset = (moveDirection - transform.position).magnitude;
            transform.position = new Vector3(moveDirection.x, transform.position.y, moveDirection.z);

            RaycastHit hit;
            //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + playerCollider.bounds.size.y, transform.position.z), transform.TransformDirection(Vector3.down) * playerCollider.bounds.size.y * 10, Color.yellow, 2);
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + playerCollider.bounds.size.y, transform.position.z), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, terrainMask))
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hit.point.y, transform.position.z), 1);
            }

            if (currentCurvedPosInfo.segmentBetweenWaypoint > 1 || currentCurvedPosInfo.segmentBetweenWaypoint < 0)
            {
                ChangeWaypointTarget();
            }

        }
        nextMoveDirection = currentCurvedPosInfo.CalculateCurvePoint(currentCurvedPosInfo.segmentBetweenWaypoint + speed * Time.deltaTime * direction * 1 / Vector3.Distance(currentCurvedPosInfo.lastWaypoint.waypointPosition.transform.position, currentCurvedPosInfo.nextWaypoint.waypointPosition.transform.position));
        nextMoveDirection = new Vector3(nextMoveDirection.x, transform.position.y, nextMoveDirection.z);
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
            if (System.Array.IndexOf(path.waypointCurves, currentCurvedPosInfo.nextWaypoint) < path.waypointCurves.Length - 1)
            {
                if (!canMove)
                    canMove = true;
                //nextWaypoint = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, nextWaypoint) + 1];
                waypointSwitch = currentCurvedPosInfo.nextWaypoint;
                currentCurvedPosInfo.nextWaypoint = currentCurvedPosInfo.lastWaypoint;
                currentCurvedPosInfo.lastWaypoint = waypointSwitch;

                currentCurvedPosInfo.segmentBetweenWaypoint = 1 - currentCurvedPosInfo.segmentBetweenWaypoint;
                Debug.Log("Swap de points waypoint cible : " + System.Array.IndexOf(path.waypointCurves, currentCurvedPosInfo.nextWaypoint));
                Debug.Log("Swap de points last waypoint : " + System.Array.IndexOf(path.waypointCurves, currentCurvedPosInfo.lastWaypoint));
            }
            else
            {
                canMove = false;
            }
        }
        //Si on va à gauche
        else if (direction < 0)
        {
            if (System.Array.IndexOf(path.waypointCurves, currentCurvedPosInfo.nextWaypoint) > 0)
            {
                if (!canMove)
                    canMove = true;
                //nextWaypoint = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, nextWaypoint) - 1];
                waypointSwitch = currentCurvedPosInfo.nextWaypoint;
                currentCurvedPosInfo.nextWaypoint = currentCurvedPosInfo.lastWaypoint;
                currentCurvedPosInfo.lastWaypoint = waypointSwitch;
                currentCurvedPosInfo.segmentBetweenWaypoint = 1 - currentCurvedPosInfo.segmentBetweenWaypoint;
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
                if (System.Array.IndexOf(path.waypointCurves, currentCurvedPosInfo.nextWaypoint) < path.waypointCurves.Length - 1)
                {
                    currentCurvedPosInfo.lastWaypoint = currentCurvedPosInfo.nextWaypoint;
                    currentCurvedPosInfo.nextWaypoint = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, currentCurvedPosInfo.nextWaypoint) + 1];
                    currentCurvedPosInfo.segmentBetweenWaypoint = 0;
                }
                break;

            case -1:
                if (System.Array.IndexOf(path.waypointCurves, currentCurvedPosInfo.lastWaypoint) > 0)
                {
                    currentCurvedPosInfo.nextWaypoint = currentCurvedPosInfo.lastWaypoint;
                    currentCurvedPosInfo.lastWaypoint = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, currentCurvedPosInfo.nextWaypoint) - 1];
                    currentCurvedPosInfo.segmentBetweenWaypoint = 1;
                }
                break;
            default:
                break;
        }
    }

    public void Respawn()
    {
        currentCurvedPosInfo.SetValues(respawnCurvedPosInfo);
        Vector3 newPos = currentCurvedPosInfo.CalculateCurvePoint(respawnCurvedPosInfo.segmentBetweenWaypoint);
        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
        Fader.Instance.respawnDelegate -= Respawn;
    }

    public int GetDirection()
    {
        return direction;
    }

    public void SaveCurrentPosInfo()
    {
        Debug.Log("Position saved");
        respawnCurvedPosInfo.lastWaypoint = currentCurvedPosInfo.lastWaypoint;
        respawnCurvedPosInfo.nextWaypoint = currentCurvedPosInfo.nextWaypoint;
        respawnCurvedPosInfo.segmentBetweenWaypoint = currentCurvedPosInfo.segmentBetweenWaypoint;
    }
}
