using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 6;
    public float SprintSpeed = 10;
    public float TurnSpeed = 15;
    public float JumpHeight = 5f;
    public float gravityValue = -9.81f;

    public Transform CameraLookAt;

    public Cinemachine.AxisState XAxis;
    public Cinemachine.AxisState YAxis;

    public List<AudioClip> walkingSounds;
    public List<AudioClip> runningSounds;

    public CinemachineCameraOffset cameraOffset;

    CharacterController characterController;

    Camera mainCamera;
    Animator animator;
    AudioSource audioSource;
    Vector3 velocity;
    bool groundedPlayer = false;

    BaseWeapon currentWeapon;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        mainCamera = Camera.main;

        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        currentWeapon = GetComponentInChildren<BaseWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!groundedPlayer)
        {
            groundedPlayer = characterController.isGrounded;
        }

        PlayerMovement();
        PlayerLook();

        if (Input.GetMouseButtonDown(0))
        {
            currentWeapon.ShootGun();
        }

        if (Input.GetMouseButton(1))
        {
            cameraOffset.m_Offset = Vector3.Lerp(cameraOffset.m_Offset, new Vector3(0, -0.8f, 2.55f), 5 * Time.deltaTime);
        }
        else
        {
            cameraOffset.m_Offset = Vector3.Lerp(cameraOffset.m_Offset, new Vector3(0, 0, 0), 5 * Time.deltaTime);
        }
    }

    void PlayerLook()
    {
        XAxis.Update(Time.deltaTime);
        YAxis.Update(Time.deltaTime);

        CameraLookAt.eulerAngles = new Vector3(YAxis.Value, XAxis.Value, 0);

        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), TurnSpeed * Time.deltaTime);
    }

    void PlayerMovement()
    {
        var hor = Input.GetAxis("Horizontal");
        var ver = Input.GetAxis("Vertical");

        var movementInput = new Vector3(hor, 0, ver);
        movementInput = Vector3.ClampMagnitude(movementInput, 1f);

        animator.SetFloat("InputX", movementInput.x);
        animator.SetFloat("InputY", movementInput.z);

        if (Input.GetKey(KeyCode.LeftShift) && ver > 0)
        {
            animator.SetBool("Sprinting", true);
            velocity.x = movementInput.x * SprintSpeed;
            velocity.z = movementInput.z * SprintSpeed;

            if (velocity.magnitude > 0)
            {

            }
        }
        else
        {
            animator.SetBool("Sprinting", false);
            velocity.x = movementInput.x * Speed;
            velocity.z = movementInput.z * Speed;
        }

        if (characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = JumpHeight;
            }
        }


        velocity.y += gravityValue * Time.deltaTime;
        velocity = transform.TransformDirection(velocity);

        characterController.Move(velocity * Time.deltaTime);
    }

    // Called from the animations
    void Footstep(float isRunning)
    {
        if (characterController.isGrounded)
        {
            if (isRunning == 1)
            {
                audioSource.PlayOneShot(runningSounds[Random.Range(0, runningSounds.Count)]);
            }
            else
            {
                audioSource.PlayOneShot(walkingSounds[Random.Range(0, walkingSounds.Count)]);
            }
        }
    }
}