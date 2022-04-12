using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Movement : MonoBehaviour
{
    private Controls controls;
    private float speed;
    [SerializeField] private float baseSpeed = 10f;
    [SerializeField] private float boostSpeed = 3f;
    private bool canDoubleJump = true;

    [SerializeField] private CinemachineImpulseSource impulseSource;
    private Camera mainCamera;
    private Animator animator;
    private Rigidbody rb;

    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private static readonly int IsJumping = Animator.StringToHash("isJumping");

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        controls = new Controls();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controls.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        controls.Disable();
    }

    void Start()
    {
        speed = baseSpeed;
        mainCamera = Camera.main;
        rb = gameObject.GetComponent<Rigidbody>();
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!controls.Player.Move.IsPressed()) return;
        Vector2 input = controls.Player.Move.ReadValue<Vector2>();
        Vector3 target = HandleInput(input);
        RotateCharacter(target);
    }

    private void FixedUpdate()
    {
        if (controls.Player.Jump.IsPressed())
        {
            if (animator.GetBool(IsJumping) && canDoubleJump)
            {
                animator.SetBool(IsJumping, false);
                animator.SetBool(IsJumping, true);
                canDoubleJump = false;
            }

            if (!animator.GetBool(IsJumping))
            {
                animator.SetBool(IsJumping, true);
            }
        }

        if (controls.Player.Run.IsPressed())
        {
            animator.SetBool(IsRunning, true);
            impulseSource.GenerateImpulse();
            speed = baseSpeed * boostSpeed;
        }
        else
        {
            animator.SetBool(IsRunning, false);
            speed = baseSpeed;
        }

        if (controls.Player.Move.IsPressed())
        {
            animator.SetBool(IsWalking, true);
            Vector2 input = controls.Player.Move.ReadValue<Vector2>();
            Vector3 target = HandleInput(input);
            MovePhysics(target);
        }
        else
        {
            animator.SetBool(IsWalking, false);
        }
    }

    private Vector3 HandleInput(Vector2 input)
    {
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = right * input.x + forward * input.y;

        return transform.position + direction * speed * Time.deltaTime;
    }

    private void MovePhysics(Vector3 target)
    {
        rb.MovePosition(target);
    }

    private void RotateCharacter(Vector3 target)
    {
        transform.rotation = Quaternion.LookRotation(target - transform.position);
    }

    public void Jump()
    {
        rb.AddForce(Vector3.up * 300);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            animator.SetBool(IsJumping, false);
            canDoubleJump = true;
        }
    }
}
