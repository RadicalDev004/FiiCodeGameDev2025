using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController Controller;
    public Transform Cam;
    public float speed = 12f;
    public float jumpForce = 5;
    public float gravity = -9.81f;
    private Animator StaffAnimator;

    public Vector3 velocity, move;
    private Vector2 MovementInput;

    public bool isGrounded;

    private Player PlayerInput;

    public Vector3 LastPos = Vector3.zero;

    private bool StopAudio = true, Jumping = false;
    public static bool isPaused = false;
    public static int SprintAdditive;

    public float RayDist;

    private float footstepTimer = 0f;
    private float footstepInterval = 0.5f;
    private bool wasGroundedLastFrame = true;

    private string walkMode = "Step";

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        PlayerInput = new();
        isPaused = false;
        StaffAnimator = Ref.PlayerBehaviour.StaffAnimator;
    }

    private void OnEnable()
    {
        PlayerInput.Enable();
    }
    private void OnDisable()
    {
        PlayerInput.Disable();
    }

    void Update()
    {
        if (isPaused)
            return;

        isGrounded = Controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
            AudioManager.Play("Jump");
        }
        if (!Input.GetKeyDown(KeyCode.Space) && Jumping && isGrounded)
        {
            Jumping = false;
            StaffAnimator.SetBool("jump", false);
        }
        if (!Jumping && !isGrounded && !StaffAnimator.GetBool("jump"))
        {
            Jumping = true;
            StaffAnimator.SetBool("jump", true);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (StaffAnimator.GetFloat("walkSpeed") == 1)
            {
                StaffAnimator.SetFloat("walkSpeed", 1.7f);
            }
            if (SprintAdditive == 1)
            {
                Tween.Value(Cam.GetComponent<Camera>().fieldOfView, 70, val => Cam.GetComponent<Camera>().fieldOfView = val, 0.2f, 0, Tween.EaseInOut);
            }
            SprintAdditive = 2;
        }
        else
        {
            if (StaffAnimator.GetFloat("walkSpeed") > 1)
            {
                StaffAnimator.SetFloat("walkSpeed", 1);
            }
            if (SprintAdditive == 2)
            {
                Tween.Value(Cam.GetComponent<Camera>().fieldOfView, 60, val => Cam.GetComponent<Camera>().fieldOfView = val, 0.2f, 0, Tween.EaseInOut);
            }
            SprintAdditive = 1;
        }

        MovementInput = PlayerInput.Main.Move.ReadValue<Vector2>();
        move = Cam.forward * MovementInput.y + Cam.right * MovementInput.x;
        move.y = 0;
        move = AdjustVelocityToSlope(move);
        Controller.Move((BossCrystal.C2 ? speed / 2 : speed) * SprintAdditive * Time.deltaTime * move);

        velocity.y += gravity * Time.deltaTime;

        if (MovementInput.x != 0 || MovementInput.y != 0)
        {
            if (isGrounded && !StaffAnimator.GetBool("walk"))
            {
                StaffAnimator.SetBool("walk", true);
            }
        }
        
        else
        {
            StaffAnimator.SetBool("walk", false);
        }

        Controller.Move(velocity * Time.deltaTime);

        if (LastPos != gameObject.transform.position)
        {
            if (!StopAudio)
            {
                StopAudio = true;
            }
        }
        else if (StopAudio || Time.timeScale == 0)
        {
            StopAudio = false;
        }

        bool isMoving = MovementInput.magnitude > 0.1f;
        footstepInterval = (Input.GetKey(KeyCode.LeftShift)) ? 0.28f : 0.5f;

        if (isGrounded && isMoving)
        {
            if (footstepInterval == 0.28f)
                walkMode = "Run";
            else
                walkMode = "Step";

            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstep(walkMode);
                footstepTimer = footstepInterval;
            }
        }

        if (!wasGroundedLastFrame && isGrounded)
        {
            AudioManager.Play("Land");
        }

        wasGroundedLastFrame = isGrounded;
    }

    public Vector3 AdjustVelocityToSlope(Vector3 velo)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit rch, RayDist))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, rch.normal);
            var adjustedVelo = slopeRotation * velo;

            if (adjustedVelo.y < 0)
            {
                return adjustedVelo;
            }
        }

        return velo;
    }

    public void Jump(float multiplier = 0)
    {
        AudioManager.Play("Jump");
        Jumping = true;
        StaffAnimator.SetBool("jump", true);
        velocity.y += Mathf.Sqrt((jumpForce + multiplier) * -3.0f * gravity);
    }

    private void PlayFootstep(string name)
    {
        int index = Random.Range(1, 6);
        AudioManager.Play(name + index);
    }
}
