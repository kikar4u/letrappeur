using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Components
    [HideInInspector] public CharacterController characterController;
    #endregion

    #region Movements
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    [HideInInspector] public bool hasMovementControls;
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
        characterController = GetComponent<CharacterController>();
        hasMovementControls = true;
        closestWayPointNode = path.waypointCurves[0];
        transform.position = closestWayPointNode.waypointPosition.transform.position;
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

        if (!hasMovementControls)
        {
            Rotate();
        }
    }
    void CharacterWalk()
    {
        if (characterController.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 0.0f);
            moveDirection *= speed;

            // Système d'actions
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        if (hasMovementControls)
        {
            characterController.Move(moveDirection * Time.deltaTime);
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

        if (characterController.isGrounded)
        {
            //If we want to move relatively to the next waypoint
            // moveDirection = new Vector3(
            //     Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * speed * (closestWayPointNode.waypointPosition.position - transform.position).normalized.x,
            //     0.0f,
            //     Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * speed * (closestWayPointNode.waypointPosition.position - transform.position).normalized.z);

            //If we want to move relatively to forward rotation
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
        }

        moveDirection.y -= gravity * Time.deltaTime;

        if (forwardWayPointAngle < moveAfterRotationDegreeThreshold)
        {
            characterController.Move(moveDirection * Time.deltaTime);
        }
        Rotate();
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
                closestWayPointNode = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, closestWayPointNode) + 1];
            }
        }
        //Si on va à gauche
        else if (direction < 0)
        {
            if (System.Array.IndexOf(path.waypointCurves, closestWayPointNode) > 0)
            {
                closestWayPointNode = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, closestWayPointNode) - 1];
            }
        }
    }

    public void Respawn()
    {
        transform.position = checkpointPosition;
    }
}
