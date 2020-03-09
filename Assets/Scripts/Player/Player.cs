using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Components
    [HideInInspector] CharacterController characterController;
    [HideInInspector] Collider playerCollider;
    [HideInInspector] public Animator animator;
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
    private CurvedPositionInfo nextCurvedPosInfo;
    private CurvedPositionInfo respawnCurvedPosInfo;
    private int direction;
    float currentCurveLength;

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
        nextCurvedPosInfo = new CurvedPositionInfo();
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

        nextCurvedPosInfo.nextWaypoint = path.waypointCurves[2];
        nextCurvedPosInfo.lastWaypoint = path.waypointCurves[1];
        nextCurvedPosInfo.segmentBetweenWaypoint = 0;
        SaveCurrentPosInfo();
        transform.position = currentCurvedPosInfo.lastWaypoint.waypointPosition.transform.position;
        Fader.Instance.respawnDelegate += Respawn;

        currentCurveLength = currentCurvedPosInfo.GetCurvedLength(0.0001f);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            raycastController.interactionAnim();
        }
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
        else if (trapperAnim.GetCurrentState() != AnimState.CLIMB && trapperAnim.GetCurrentState() != AnimState.BREATH && trapperAnim.GetCurrentState() != AnimState.CHOP)
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

            if (Input.GetAxisRaw("Horizontal") != 0f && (trapperAnim.GetCurrentState() == AnimState.WALK || trapperAnim.GetCurrentState() == AnimState.IDLE))
            {
                currentCurvedPosInfo.segmentBetweenWaypoint += Mathf.Abs(Input.GetAxis("Horizontal")) * direction * _speed * Time.deltaTime / currentCurveLength;
                Debug.Log(currentCurvedPosInfo.segmentBetweenWaypoint);
                Debug.Log("Value : " + Mathf.Abs(Input.GetAxis("Horizontal")) * direction * _speed * Time.deltaTime / currentCurveLength);
                if (trapperAnim.GetCurrentState() != AnimState.WALK)
                {
                    trapperAnim.SetAnimState(AnimState.WALK);
                }
            }
            else if (trapperAnim.GetCurrentState() == AnimState.CLIMB || trapperAnim.GetCurrentState() == AnimState.PASSIVE_WALK)
            {
                currentCurvedPosInfo.segmentBetweenWaypoint += direction * _speed * Time.deltaTime / currentCurveLength;
            }

            currentCurvedPosInfo.segmentBetweenWaypoint = Mathf.Clamp01(currentCurvedPosInfo.segmentBetweenWaypoint);
            moveDirection = currentCurvedPosInfo.CalculateCurvePoint(currentCurvedPosInfo.segmentBetweenWaypoint);
            moveDirection = new Vector3(moveDirection.x, transform.position.y, moveDirection.z);

            Rotate(moveDirection);
            movementOffset = (moveDirection - transform.position).magnitude;
            //Debug.Log(movementOffset);
            transform.position = Vector3.Lerp(transform.position, new Vector3(moveDirection.x, transform.position.y, moveDirection.z), 0.8f);

            RaycastHit hit;
            //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + playerCollider.bounds.size.y, transform.position.z), transform.TransformDirection(Vector3.down) * playerCollider.bounds.size.y * 10, Color.yellow, 2);
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + playerCollider.bounds.size.y, transform.position.z), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, terrainMask))
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hit.point.y, transform.position.z), 1);
            }

            if (currentCurvedPosInfo.segmentBetweenWaypoint >= 1f || currentCurvedPosInfo.segmentBetweenWaypoint <= 0f)
            {
                ChangeWaypointTarget();
            }

            SetNextMoveDir(_speed);

        }

        //Debug.Log(nextMoveDirection - moveDirection);
        //Rotate(nextMoveDirection);
    }

    private Vector3 SetNextMoveDir(float _speed)
    {
        if (currentCurvedPosInfo.segmentBetweenWaypoint + (direction * _speed * Time.deltaTime / currentCurveLength) > 1)
        {
            nextMoveDirection = nextCurvedPosInfo.CalculateCurvePoint(direction * _speed * Time.deltaTime / nextCurvedPosInfo.GetCurvedLength(0.02f));
        }
        else if (currentCurvedPosInfo.segmentBetweenWaypoint + (direction * _speed * Time.deltaTime / currentCurveLength) < 0)
        {
            nextMoveDirection = nextCurvedPosInfo.CalculateCurvePoint(1 - (direction * _speed * Time.deltaTime / nextCurvedPosInfo.GetCurvedLength(0.02f)));
        }

        else
        {
            nextMoveDirection = currentCurvedPosInfo.CalculateCurvePoint(currentCurvedPosInfo.segmentBetweenWaypoint + (direction * _speed * Time.deltaTime / currentCurveLength));
            nextMoveDirection = new Vector3(nextMoveDirection.x, transform.position.y, nextMoveDirection.z);
        }

        return nextMoveDirection;
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

                    if (System.Array.IndexOf(path.waypointCurves, nextCurvedPosInfo.nextWaypoint) < path.waypointCurves.Length - 1)
                    {
                        nextCurvedPosInfo.lastWaypoint = currentCurvedPosInfo.nextWaypoint;
                        nextCurvedPosInfo.nextWaypoint = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, nextCurvedPosInfo.nextWaypoint) + 1];
                    }
                }
                break;

            case -1:
                if (System.Array.IndexOf(path.waypointCurves, currentCurvedPosInfo.lastWaypoint) > 0)
                {
                    currentCurvedPosInfo.nextWaypoint = currentCurvedPosInfo.lastWaypoint;
                    currentCurvedPosInfo.lastWaypoint = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, currentCurvedPosInfo.nextWaypoint) - 1];
                    currentCurvedPosInfo.segmentBetweenWaypoint = 1;

                    if (System.Array.IndexOf(path.waypointCurves, nextCurvedPosInfo.lastWaypoint) > 0)
                    {
                        nextCurvedPosInfo.nextWaypoint = currentCurvedPosInfo.lastWaypoint;
                        nextCurvedPosInfo.lastWaypoint = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, nextCurvedPosInfo.lastWaypoint) - 1];
                    }
                }
                break;
            default:
                break;
        }
        currentCurveLength = currentCurvedPosInfo.GetCurvedLength(0.002f);
    }

    public void Respawn()
    {
        trapperAnim.SetAnimState(AnimState.IDLE);
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
