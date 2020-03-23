﻿using System.Collections;
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
    [HideInInspector] public AudioSource audioSource;
    #endregion

    #region Movements
    [Range(0, 100)]
    public float speed;
    public float gravity = 20.0f;
    //Booléens de contrôles du mouvement
    // @ canmove : Le joueur est à l'extrémité du chemin
    // @ inCinematic : Le joueur est dans une cinématique
    // @ hasMovementControls : Le joueur peut contrôler le déplacement avec le joystick
    // @ blocked : Le joueur est bloqué par un obstacle
    [HideInInspector] public bool canMove, inCinematic, hasMovementControls, blocked;
    private PathCurve path;
    //CurvedPositionInfo contient les informations nécessaires pour connaître une position sur la courbe : 
    //2 WaypointCurve du chemin et un float {0,1} interpolant la position sur la courbe
    private CurvedPositionInfo currentCurvedPosInfo;
    //private CurvedPositionInfo nextCurvedPosInfo;
    [HideInInspector] public CurvedPositionInfo respawnCurvedPosInfo;
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
        #region Components
        playerCollider = GetComponent<Collider>();
        raycastController = GetComponent<InteractionRaycast>();
        trapperAnim = GetComponent<TrapperAnim>();
        path = GameObject.FindGameObjectWithTag("Path").GetComponent<PathCurve>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        terrainMask = LayerMask.GetMask("Ground");
        audioSource = GetComponent<AudioSource>();
        #endregion
        //Initialisation des informations des waypoints
        currentCurvedPosInfo = path.GetCurvePosInfoAtIndex(0);
        respawnCurvedPosInfo = path.GetCurvePosInfoAtIndex(0);

        #region MoveControl
        hasMovementControls = true;
        canMove = true;
        inCinematic = false;
        direction = 0;
        #endregion

        SaveCurrentPosInfo();
        transform.position = currentCurvedPosInfo.lastWaypoint.waypointPosition.transform.position;
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
                }
                else
                {
                    //SwapWaypointTarget(1);
                }
            }
        }
        else if (trapperAnim.GetCurrentState() != AnimState.CLIMB && trapperAnim.GetCurrentState() != AnimState.BREATH && trapperAnim.GetCurrentState() != AnimState.CHOP)
        {
            movementOffset = 0;
            trapperAnim.SetAnimState(AnimState.IDLE);
        }

        if (!characterController.isGrounded)
        {
            characterController.Move(new Vector3(0, -gravity * Time.deltaTime, 0));
        }

        forwardWayPointAngle = Vector3.Angle(new Vector3(nextMoveDirection.x - transform.position.x, 0f, nextMoveDirection.z - transform.position.z), new Vector3(transform.forward.x, 0f, transform.forward.z));


        RaycastHit hit;
        //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + playerCollider.bounds.size.y, transform.position.z), transform.TransformDirection(Vector3.down) * playerCollider.bounds.size.y * 10, Color.yellow, 2);
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + playerCollider.bounds.size.y, transform.position.z), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, terrainMask))
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hit.point.y, transform.position.z), 1);
        }
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
    private float CalculateSpeedOnCurve(float _speed)
    {
        return direction * _speed * Time.deltaTime / currentCurvedPosInfo.GetCurvedLength();
    }

    public void WalkFollowingPath(float _speed)
    {
        if (forwardWayPointAngle < moveAfterRotationDegreeThreshold && !blocked)
        {
            if (Input.GetAxisRaw("Horizontal") != 0f && (trapperAnim.GetCurrentState() == AnimState.WALK || trapperAnim.GetCurrentState() == AnimState.IDLE))
            {
                currentCurvedPosInfo.segmentBetweenWaypoint += Mathf.Abs(Input.GetAxis("Horizontal")) * CalculateSpeedOnCurve(_speed);

                //Debug.Log(currentCurvedPosInfo.segmentBetweenWaypoint);
                //Debug.Log("Value : " + Mathf.Abs(Input.GetAxisRaw("Horizontal")) * CalculateSpeedOnCurve(_speed));
                if (trapperAnim.GetCurrentState() != AnimState.WALK)
                {
                    trapperAnim.SetAnimState(AnimState.WALK);
                }
            }
            else if (trapperAnim.GetCurrentState() == AnimState.CLIMB || trapperAnim.GetCurrentState() == AnimState.PASSIVE_WALK)
            {
                currentCurvedPosInfo.segmentBetweenWaypoint += CalculateSpeedOnCurve(_speed);
            }

            if (currentCurvedPosInfo.segmentBetweenWaypoint > 1f || currentCurvedPosInfo.segmentBetweenWaypoint < 0f)
            {
                ChangeWaypointTarget(direction);
            }
            currentCurvedPosInfo.segmentBetweenWaypoint = Mathf.Clamp01(currentCurvedPosInfo.segmentBetweenWaypoint);

            moveDirection = currentCurvedPosInfo.CalculateCurvePoint(currentCurvedPosInfo.segmentBetweenWaypoint);
            //Debug.Log("Magnitude : " + (new Vector3(moveDirection.x, transform.position.y, moveDirection.z) - transform.position).magnitude);
            moveDirection = new Vector3(moveDirection.x, transform.position.y, moveDirection.z);
            Rotate(moveDirection);
            movementOffset = Vector3.Distance(moveDirection, transform.position);
            transform.position = new Vector3(moveDirection.x, transform.position.y, moveDirection.z);

        }

        nextMoveDirection = SetNextMoveDir(_speed);

        Rotate(nextMoveDirection);
    }

    private Vector3 SetNextMoveDir(float _speed)
    {
        nextMoveDirection = currentCurvedPosInfo.CalculateCurvePoint(currentCurvedPosInfo.segmentBetweenWaypoint + (direction * _speed * Time.deltaTime / currentCurvedPosInfo.GetCurvedLength()));
        nextMoveDirection = new Vector3(nextMoveDirection.x, transform.position.y, nextMoveDirection.z);

        return nextMoveDirection;
    }


    public void Rotate(Vector3 target)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,
        Quaternion.LookRotation(new Vector3(target.x - transform.position.x, 0f, target.z - transform.position.z)), rotationSmoothness);
    }

    void ChangeWaypointTarget(int _direction)
    {
        //Actualise le waypoint suivant et précédent
        switch (_direction)
        {
            case 1:
                if (System.Array.IndexOf(path.waypointCurves, currentCurvedPosInfo.nextWaypoint) < path.waypointCurves.Length - 1)
                {
                    if (!canMove)
                        canMove = true;

                    currentCurvedPosInfo = path.GetCurvePosInfoAtIndex(currentCurvedPosInfo.id + 1);
                    currentCurvedPosInfo.segmentBetweenWaypoint = 0f;
                }
                else
                {
                    canMove = false;
                }
                break;

            case -1:
                if (System.Array.IndexOf(path.waypointCurves, currentCurvedPosInfo.lastWaypoint) > 0)
                {
                    if (!canMove)
                        canMove = true;

                    currentCurvedPosInfo = path.GetCurvePosInfoAtIndex(currentCurvedPosInfo.id - 1);
                    currentCurvedPosInfo.segmentBetweenWaypoint = 1f;
                }
                else
                {
                    canMove = false;
                }
                break;
            default:
                break;
        }

    }

    public void Respawn()
    {
        hasMovementControls = true;
        trapperAnim.SetAnimState(AnimState.IDLE);
        currentCurvedPosInfo = respawnCurvedPosInfo;

        Debug.Log("Respawn curve ID" + currentCurvedPosInfo.id);
        Debug.Log("Respawn curve segment" + currentCurvedPosInfo.segmentBetweenWaypoint);
        Vector3 newPos = currentCurvedPosInfo.CalculateCurvePoint(currentCurvedPosInfo.segmentBetweenWaypoint);
        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);

        if (Fader.Instance.fadeOutDelegate != null)
            Fader.Instance.fadeOutDelegate -= Respawn;
    }

    public int GetDirection()
    {
        return direction;
    }

    public void SaveCurrentPosInfo()
    {
        //Sauvegarde la position actuelle du joueur
        respawnCurvedPosInfo = currentCurvedPosInfo;
        Debug.Log("Saved curve ID" + respawnCurvedPosInfo.id);
        respawnCurvedPosInfo.SetSegmenentPoint(currentCurvedPosInfo.segmentBetweenWaypoint);
        Debug.Log("Saved curve segment" + respawnCurvedPosInfo.segmentBetweenWaypoint);
    }
}
