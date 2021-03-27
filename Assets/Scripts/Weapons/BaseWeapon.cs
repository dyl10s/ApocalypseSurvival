using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour
{
    public float FireRate = 1f;
    public float Damage = 1f;
    public float Range = 100f;

    public AudioClip GunShot;

    public Transform FireFrom;
    public GameObject Bullet;
    
    Vector3 fireAt = Vector3.zero;
    Camera mainCamera;
    float timeSinceShot = 0;
    AudioSource audioSouce;

    void Start()
    {
        mainCamera = Camera.main;
        audioSouce = GetComponent<AudioSource>();
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
    }

    public void ShootGun()
    {
        if (timeSinceShot > FireRate)
        {
            audioSouce.PlayOneShot(GunShot);
            var newBullet = Instantiate(Bullet, FireFrom.position, FireFrom.rotation);
            newBullet.transform.LookAt(fireAt);
            var shootVelocity = newBullet.transform.TransformDirection(new Vector3(0, 0, 500));
            newBullet.GetComponent<Rigidbody>().AddForce(shootVelocity, ForceMode.VelocityChange);

            timeSinceShot = 0;
        }
    }
}
