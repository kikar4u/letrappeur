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
    #endregion

    #region Stats

    #endregion

    #region Checkpoint
    public Vector3 checkpointPosition;
    #endregion
    private Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        hasMovementControls = true;
    }

    void Update()
    {
        CharacterWalk();
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

    public void Respawn()
    {
        transform.position = checkpointPosition;
    }
}
