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
    #endregion

    #region Stats

    #endregion

    #region Checkpoint
    public Vector3 checkpointPosition;
    #endregion
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        direction = 0;
        path = GameObject.FindGameObjectWithTag("Path").GetComponent<PathCurve>();
        characterController = GetComponent<CharacterController>();
        hasMovementControls = true;
        closestWayPointNode = path.waypointCurves[0];

        transform.position = closestWayPointNode.waypointPosition.position;
    }

    void Update()
    {
        //CharacterWalk();
        CharacterWalkFollowingPath();
        if (Input.GetKeyDown("p"))
        {
            Respawn();
        }
        gameObject.GetComponent<InteractionRaycast>().interactionAnim();

    }
    void CharacterWalk()
    {
        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 0.0f);
            moveDirection *= speed;

            // Système d'actions
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        if (hasMovementControls)
        {
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    void CharacterWalkFollowingPath()
    {
        if (transform.position.x < (closestWayPointNode.waypointPosition.position.x + 0.2f) && transform.position.x > (closestWayPointNode.waypointPosition.position.x - 0.2f)
        || Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")) != 0 && Mathf.RoundToInt(Input.GetAxisRaw("Horizontal")) != direction)
        {
            direction = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
            UpdateWayPointTarget(direction);
        }

        moveDirection = new Vector3();
        if (characterController.isGrounded)
        {
            //If we want to move relatively to the next waypoint
            // moveDirection = new Vector3(
            //     Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * speed * (closestWayPointNode.waypointPosition.position - transform.position).normalized.x,
            //     0.0f,
            //     Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * speed * (closestWayPointNode.waypointPosition.position - transform.position).normalized.z);

            //If we want to move relatively to forward rotation
            moveDirection = new Vector3(
               Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * speed * transform.forward.x,
                0.0f,
               Mathf.Abs(Input.GetAxis("Horizontal")) * Time.deltaTime * speed * transform.forward.z);

            moveDirection *= speed;
        }

        moveDirection.y -= gravity * Time.deltaTime;

        if (hasMovementControls && direction != 0)
        {
            characterController.Move(moveDirection * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(closestWayPointNode.waypointPosition.position.x - transform.position.x, 0f, closestWayPointNode.waypointPosition.position.z - transform.position.z)), rotationSmoothness);
        }

    }

    // direction > 0 = right
    void UpdateWayPointTarget(int direction)
    {
        //If we go right
        if (direction > 0)
        {
            closestWayPointNode = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, closestWayPointNode) + 1];
            //For all waypoints after the current one
            for (int i = System.Array.IndexOf(path.waypointCurves, closestWayPointNode); i < path.waypointCurves.Length; i++)
            {
                //Check the closest one
                if ((path.waypointCurves[i].waypointPosition.position - transform.position).magnitude < (closestWayPointNode.waypointPosition.position - transform.position).magnitude)
                {
                    closestWayPointNode = path.waypointCurves[i];
                    Debug.Log("Go right & closest waypoint = " + closestWayPointNode.waypointPosition.position + " positined at :" + closestWayPointNode.waypointPosition.position);
                }
            }
        }
        //If we go left
        else if (direction < 0)
        {
            closestWayPointNode = path.waypointCurves[System.Array.IndexOf(path.waypointCurves, closestWayPointNode) - 1];
            //For all waypoints before the current one
            for (int i = System.Array.IndexOf(path.waypointCurves, closestWayPointNode); i > 0; i--)
            {
                if ((path.waypointCurves[i].waypointPosition.position - transform.position).magnitude < (closestWayPointNode.waypointPosition.position - transform.position).magnitude)
                {
                    closestWayPointNode = path.waypointCurves[i];
                    Debug.Log("Go left & closest waypoint = " + closestWayPointNode.waypointPosition.position + " positined at :" + closestWayPointNode.waypointPosition.position);
                }
            }
        }
    }

    public void Respawn()
    {
        transform.position = checkpointPosition;
    }
}
