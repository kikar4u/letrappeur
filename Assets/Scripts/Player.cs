﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Components
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] Animator animator;
    #endregion

    #region Movements
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    [HideInInspector] public bool hasMovementControls;
    [HideInInspector] public bool canMove;
    private PathCurve path;
    private WaypointCurve closestWayPointNode;
    private int direction;
    [Range(0, 1)]
    [Tooltip("1 = Tourne instantanément ; 0 = Tourne pas")]
    [SerializeField] private float rotationSmoothness;
    [Tooltip("Angle entre le forward du personnage avec la position du prochain waypoint sous lequel le joueur pourra bouger")]
    [Range(0, 180)]
    [SerializeField] private float moveAfterRotationDegreeThreshold;
    private float forwardWayPointAngle;
    private Vector3 moveDirection;
    #endregion

    #region Checkpoint
    public Vector3 checkpointPosition;
    #endregion

    void Start()
    {
        direction = 0;
        path = GameObject.FindGameObjectWithTag("Path").GetComponent<PathCurve>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        hasMovementControls = true;
        canMove = true;
        closestWayPointNode = path.waypointCurves[0];
        transform.position = new Vector3(closestWayPointNode.waypointPosition.transform.position.x, transform.position.y, closestWayPointNode.waypointPosition.transform.position.z);
    }

    void Update()
    {
        //CharacterWalk();
        gameObject.GetComponent<InteractionRaycast>().interactionAnim();
        if (hasMovementControls)
        {
            WalkFollowingPath(speed);
        }
        forwardWayPointAngle = Vector3.Angle(new Vector3(closestWayPointNode.waypointPosition.transform.position.x - transform.position.x, 0f, closestWayPointNode.waypointPosition.transform.position.z - transform.position.z), new Vector3(transform.forward.x, 0f, transform.forward.z));

        if (Input.GetKeyDown("p"))
        {
            Respawn();
        }

        if (!hasMovementControls && canMove)
        {
            Rotate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Waypoint")
        {
            if (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")) != 0)
            {
                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    direction = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                    UpdateWayPointTarget(direction);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Waypoint")
        {
            if (other.gameObject == path.waypointCurves[0].waypointPosition && direction == -1 || other.gameObject == path.waypointCurves[path.waypointCurves.Length - 1].waypointPosition && direction == 1)
            {
                canMove = false;
            }
        }
    }

    public void WalkFollowingPath(float _speed)
    {
        if (Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")) != 0 && Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")) != direction)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 && hasMovementControls)
            {
                direction = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                UpdateWayPointTarget(direction);
            }
            else if (!hasMovementControls)
            {
                direction = 1;
                UpdateWayPointTarget(direction);
            }
        }

        if (characterController.isGrounded && canMove)
        {
            //Si on veut bouger relativement au prochain waypoint
            // moveDirection = new Vector3(
            //     Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * speed * (closestWayPointNode.waypointPosition.position - transform.position).normalized.x,
            //     0.0f,
            //     Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * speed * (closestWayPointNode.waypointPosition.position - transform.position).normalized.z);

            //Si on veut bouger en fonction de sa rotation
            if (hasMovementControls)
            {
                moveDirection = new Vector3(
                    Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * transform.forward.x,
                    0.0f,
                    Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * transform.forward.z);

            }
            else
            {
                moveDirection = new Vector3(
                    Time.deltaTime * transform.forward.x,
                    0.0f,
                    Time.deltaTime * transform.forward.z);
            }

            moveDirection *= _speed;
            Rotate();
        }
        moveDirection.y -= gravity * Time.deltaTime;

        if (forwardWayPointAngle < moveAfterRotationDegreeThreshold)
        {
            characterController.Move(moveDirection * Time.deltaTime);
            if (Input.GetAxis("Horizontal") != 0f)
            {
                animator.SetBool("Walk", true);
                Debug.Log("Je marche !");
            }
        }
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            animator.SetBool("Walk", false);
            Debug.Log("Je ne marche pu...");
        }

    }

    public void Rotate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,
        Quaternion.LookRotation(new Vector3(closestWayPointNode.waypointPosition.transform.position.x - transform.position.x, 0f, closestWayPointNode.waypointPosition.transform.position.z - transform.position.z)),
        rotationSmoothness);
    }

    // direction > 0 = right
    void UpdateWayPointTarget(int direction)
    {
        //Si on va à droite
        if (direction > 0)
        {
            //On verifie qu'on ne soit pas à une extrémité
            if (System.Array.IndexOf(path.waypointCurves, closestWayPointNode) < path.waypointCurves.Length - 1)
            {
                if (!canMove)
                    canMove = true;
                closestWayPointNode = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, closestWayPointNode) + 1];
            }
            else
            {
                Debug.Log("On est à une limite");
                canMove = false;
            }
        }
        //Si on va à gauche
        else if (direction < 0)
        {
            if (System.Array.IndexOf(path.waypointCurves, closestWayPointNode) > 0)
            {
                if (!canMove)
                    canMove = true;
                closestWayPointNode = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, closestWayPointNode) - 1];
            }
            else
            {
                Debug.Log("On est à une limite");
                canMove = false;
            }
        }
    }

    public void Respawn()
    {
        transform.position = checkpointPosition;
    }
}
