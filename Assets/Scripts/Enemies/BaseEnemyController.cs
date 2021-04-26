using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using TMPro;
using System.Threading;

public class BaseEnemyController : MonoBehaviour
{
    public int Health = 10;
    public float SightDistance = 10f;
    public float AttackDistance = 1.5f;

    public float Speed = 1f;
    public static int KillScore = 0;

    Transform PlayerEyes;
    public Transform EyeLocation;
    public ParticleSystem BloodEffect;
    public int firstDieCall = 0;
    public Collider AttackCollider;

    bool dieing = false;
    bool foundPlayer = false;

    Collider col;
    Animator anim;
    Rigidbody rb;

    ParticleSystem createdBloodEffect;

    Vector3 lastCollisionPoint;



    GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        PlayerEyes = GameObject.Find("CameraTarget").transform;

        AttackCollider.enabled = false;
        
        setRagdollState(false);
        createdBloodEffect = Instantiate(BloodEffect, transform);

        

        Player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!AttackPlayer())
        {
            if ((foundPlayer || CheckForPlayer()) && !dieing)
            {
                RotateToPlayer();
                MoveToPlayer();
            }
        }
        else
        {
            RotateToPlayer();
        }

        if (Health <= 0)
        {
            dieing = true;
            Die();

            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.mass = 1;
                rb.AddExplosionForce(.2f, lastCollisionPoint, 1, .5f, ForceMode.Impulse);
            }
        }
        
    }

    bool AttackPlayer()
    {
        if(anim.GetBool("IsAttacking") == false)
        {
            RaycastHit hit;
            if (Physics.Raycast(EyeLocation.position, PlayerEyes.position - EyeLocation.position, out hit, AttackDistance))
            {
                if (hit.transform.tag == "Player")
                {
                    anim.SetBool("IsAttacking", true);
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    void RotateToPlayer()
    {
        var lookPos = PlayerEyes.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 100);
    }

    void MoveToPlayer()
    {
        transform.position += transform.forward * Time.deltaTime * Speed;
    }

    bool CheckForPlayer()
    {
        RaycastHit hit;
        if(Physics.Raycast(EyeLocation.position, PlayerEyes.position - EyeLocation.position, out hit, SightDistance))
        {
            if(hit.transform.tag == "Player")
            {
                anim.SetBool("IsWalking", true);
                foundPlayer = true;
                return true;
            }
        }

        anim.SetBool("IsWalking", false);
        return false;
    }

    public void TakeDamage(int damage, Collision collision)
    {
        
        Health -= damage;
        lastCollisionPoint = collision.GetContact(0).point;

        createdBloodEffect.transform.position = lastCollisionPoint;
        createdBloodEffect.transform.rotation = Quaternion.FromToRotation(Vector3.up, collision.GetContact(0).normal);

        createdBloodEffect.Emit(10);
    }

    void Die()
    {
        if (firstDieCall == 0)
        {
            Player.SendMessage("incrementScore", SendMessageOptions.DontRequireReceiver);
            firstDieCall++;
        }
        anim.enabled = false;
        setRagdollState(true);        
        Destroy(this.gameObject, 15);

    }

    void StartAttack()
    {
        AttackCollider.enabled = true;
    }

    void EndAttack()
    {
        AttackCollider.enabled = false;
    }

    void AttackOver()
    {
        anim.SetBool("IsAttacking", false);
    }

    // For ragdoll when dead
    void setRagdollState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach(var r in rigidbodies)
        {
            r.isKinematic = !state;
        }

        foreach(var c in colliders)
        {
            c.enabled = state;
        }

        rb.isKinematic = false;
        col.enabled = !state;
    }


}
