using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BaseWeapon : MonoBehaviour
{
    public float FireRate = 1f;
    public float Damage = 1f;
    public float Range = 100f;

    public bool FullAuto = false;
    public int BulletsPerTriggerPress = 1;

    public AudioClip GunShot;

    public Transform FireFrom;
    public GameObject Bullet;
    public ParticleSystem MuzzleFlash;
    
    public AnimationClip GunLocationSetup;

    Vector3 fireAt = Vector3.zero;
    Camera mainCamera;
    float timeSinceShot = 0;
    AudioSource audioSouce;
    Animator anim;
    int bulletsShotSinceTriggerPressed = 0;

    bool triggerDown = false;

    void Start()
    {
        mainCamera = Camera.main;
        audioSouce = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        GunShot.LoadAudioData();
    }

    void Update()
    {
        timeSinceShot += Time.deltaTime;
        RaycastHit centerOfScreen;
        if(Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out centerOfScreen, Range))
        {
            fireAt = centerOfScreen.point;
        }
        else
        {
            fireAt = mainCamera.transform.position + mainCamera.transform.forward * Range;
        }

        if (triggerDown)
        {
            ShootGun();
        }
    }

    void ShootGun()
    {
        if (timeSinceShot > FireRate && (bulletsShotSinceTriggerPressed < BulletsPerTriggerPress || FullAuto))
        {
            anim.Play("Shoot");
            audioSouce.PlayOneShot(GunShot);
            MuzzleFlash.Play();

            var newBullet = Instantiate(Bullet, FireFrom.position, FireFrom.rotation);
            newBullet.transform.LookAt(fireAt);
            var shootVelocity = newBullet.transform.TransformDirection(new Vector3(0, 0, 500));
            newBullet.GetComponent<Rigidbody>().AddForce(shootVelocity, ForceMode.VelocityChange);

            bulletsShotSinceTriggerPressed += 1;
            timeSinceShot = 0;
        }
    }

    public void PullTrigger()
    {
        triggerDown = true;
    }

    public void ReleaseTrigger()
    {
        triggerDown = false;
        bulletsShotSinceTriggerPressed = 0;
    }
}
