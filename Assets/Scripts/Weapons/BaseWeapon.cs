using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;
using TMPro;

public class BaseWeapon : MonoBehaviour
{
    public float FireRate = 1f;
    public float Damage = 1f;
    public float Range = 100f;

    public int ClipSize = 10;
    public float ReloadTime = 1.5f;

    public bool FullAuto = false;
    public int BulletsPerTriggerPress = 1;

    public AudioClip GunShot;

    public Transform FireFrom;
    public GameObject Bullet;
    public ParticleSystem MuzzleFlash;
    
    public AnimationClip GunLocationSetup;
    public AudioClip ReloadAudio;

    Vector3 fireAt = Vector3.zero;
    Camera mainCamera;
    float timeSinceShot = 0;
    AudioSource audioSouce;
    Animator anim;
    Image reloadAnimation;
    TMP_Text uiAmmoCount;

    int bulletsShotSinceTriggerPressed = 0;

    public int bulletsLeft = 10;
    bool reloading = false;
    float reloadingTime = 0f;

    bool triggerDown = false;

    void Start()
    {
        reloadAnimation = GameObject.Find("ReloadAnimation").GetComponent<Image>();
        reloadAnimation.fillAmount = 0f;

        mainCamera = Camera.main;
        audioSouce = GetComponent<AudioSource>();
        uiAmmoCount = GameObject.Find("AmmoCounter").GetComponent<TMP_Text>();
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

        if(Input.GetKeyDown(KeyCode.R) && !reloading && bulletsLeft < ClipSize)
        {
            StartReload();
        }

        if (reloading)
        {
            Reload();
        }
        uiAmmoCount.text = string.Concat("Ammo: ", bulletsLeft.ToString());
    }

    void StartReload()
    {
        reloading = true;
        reloadingTime = 0f;
    }

    void ShootGun()
    {
        if (timeSinceShot > FireRate && (bulletsShotSinceTriggerPressed < BulletsPerTriggerPress || FullAuto))
        {
            if(bulletsLeft > 0 && !reloading)
            {
                anim.Play("Shoot");
                audioSouce.PlayOneShot(GunShot);
                MuzzleFlash.Emit(100);

                var newBullet = Instantiate(Bullet, FireFrom.position, FireFrom.rotation);
                newBullet.transform.LookAt(fireAt);
                var shootVelocity = newBullet.transform.TransformDirection(new Vector3(0, 0, 500));
                newBullet.GetComponent<Rigidbody>().AddForce(shootVelocity, ForceMode.VelocityChange);

                bulletsShotSinceTriggerPressed += 1;
                timeSinceShot = 0;
                bulletsLeft -= 1;
            }
            else
            {
                if(!reloading)
                {
                    StartReload();
                }
            }
        }
    }

    void Reload()
    {
        if(reloadingTime == 0)
        {
            audioSouce.clip = ReloadAudio;
            audioSouce.Play();
        }

        reloadingTime += Time.deltaTime;
        reloadAnimation.fillAmount = reloadingTime / ReloadTime;

        if (reloadingTime >= ReloadTime)
        {
            reloadAnimation.fillAmount = 0;
            reloading = false;
            bulletsLeft = ClipSize;
            audioSouce.Stop();
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
