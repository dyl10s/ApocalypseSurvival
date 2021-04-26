using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float Speed = 6;
    public float SprintSpeed = 10;
    public float TurnSpeed = 15;
    public float JumpHeight = 5f;
    public float gravityValue = -9.81f;
    
    public int Health = 100;
    public int killScore = 0;

    public Transform CameraLookAt;

    public Cinemachine.AxisState XAxis;
    public Cinemachine.AxisState YAxis;

    public List<AudioClip> walkingSounds;
    public List<AudioClip> runningSounds;

    public Transform weaponLeftGrip;
    public Transform weaponRightGrip;
    public Transform weaponParent;
    public Transform weaponLocation;

    public CinemachineCameraOffset cameraOffset;

    public Rig handRig; 

    CharacterController characterController;

    Camera mainCamera;
    Animator animator;
    AnimatorOverrideController animOverrides;
    AudioSource audioSource;
    Vector3 velocity;
    Collider col;
    bool groundedPlayer = false;

    BaseWeapon currentWeapon;

    TMP_Text Score;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        mainCamera = Camera.main;

        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        animOverrides = (AnimatorOverrideController)animator.runtimeAnimatorController;
        characterController = GetComponent<CharacterController>();
        
        col = GetComponent<Collider>();
        

        var startingGun = GetComponentInChildren<BaseWeapon>();
        currentWeapon = startingGun;
        Equip(startingGun);

        Score = GameObject.Find("Score").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!groundedPlayer)
        {
            groundedPlayer = characterController.isGrounded;
        }

        if(Health <= 0)
        {
            SceneManager.LoadScene("DeadScene");
        }

        PlayerMovement();
        PlayerLook();

        if (currentWeapon != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentWeapon.PullTrigger();
            }

            if (Input.GetMouseButtonUp(0))
            {
                currentWeapon.ReleaseTrigger();
            }
        }
        

        if (Input.GetMouseButton(1))
        {
            cameraOffset.m_Offset = Vector3.Lerp(cameraOffset.m_Offset, new Vector3(0, -0.8f, 2.55f), 5 * Time.deltaTime);
        }
        else
        {
            cameraOffset.m_Offset = Vector3.Lerp(cameraOffset.m_Offset, new Vector3(0, 0, 0), 5 * Time.deltaTime);
        }
        Score.text = string.Concat("Score: ", killScore);
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

    public void Equip(BaseWeapon newWeapon)
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        if (newWeapon == null)
        {
            handRig.weight = 0;
            animator.SetLayerWeight(1, 0);
            animOverrides["Equip_None"] = null;
            currentWeapon = null;
        }
        else
        {
            var weaponToEquip = Instantiate(newWeapon.gameObject, weaponLocation);
            currentWeapon = weaponToEquip.GetComponent<BaseWeapon>();
            handRig.weight = 1;
            animator.SetLayerWeight(1, 1);
            animOverrides["Equip_None"] = currentWeapon.GunLocationSetup;
        }

        
    }

#if UNITY_EDITOR
    [ContextMenu("Toggle Animation")]
    void DisableAnimation()
    {
        animator.enabled = !animator.enabled;
    }

    // This is ued in the editor to setup new weapon poses
    [ContextMenu("Save Pose To Animation")]
    void SaveWeaponPose()
    {
        GameObjectRecorder recorder = new GameObjectRecorder(gameObject);
        recorder.BindComponentsOfType<Transform>(weaponParent.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponLocation.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponLeftGrip.gameObject, false);
        recorder.BindComponentsOfType<Transform>(weaponRightGrip.gameObject, false);
        recorder.TakeSnapshot(0);
        recorder.SaveToClip(currentWeapon.GunLocationSetup);
        UnityEditor.AssetDatabase.SaveAssets();
    }
#endif

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "AttackCollider")
        {
            Health -= 8;
        }
    }

    public void incrementScore()
    {
        killScore++;
    }
}

  