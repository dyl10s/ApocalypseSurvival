using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BaseEnemyController : MonoBehaviour
{
    public int Health = 10;
    public float SightDistance = 10f;
    public float Speed = 10f;

    public Transform PlayerEyes;
    public Transform EyeLocation;
    public ParticleSystem BloodEffect;

    bool dieing = false;

    Collider col;
    Animator anim;
    Rigidbody rb;

    ParticleSystem createdBloodEffect;

    Vector3 lastCollisionPoint;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        setRagdollState(false);
        createdBloodEffect = Instantiate(BloodEffect);
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckForPlayer() && !dieing)
        {
            MoveToPlayer();
        }

        if (Health <= 0)
        {
            dieing = true;
            Die();

            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(.2f, lastCollisionPoint, 1, .5f, ForceMode.Impulse);
            }
        }
    }

    void MoveToPlayer()
    {
        var oldRotation = transform.rotation;
        transform.LookAt(PlayerEyes.transform);        
        transform.rotation = new Quaternion(oldRotation.x, transform.rotation.y, oldRotation.z, oldRotation.w);
        rb.AddRelativeForce(new Vector3(0, 0, Speed) * Time.deltaTime, ForceMode.Impulse);
    }

    bool CheckForPlayer()
    {
        RaycastHit hit;
        if(Physics.Raycast(EyeLocation.position, PlayerEyes.position - EyeLocation.position, out hit, SightDistance))
        {
            if(hit.transform.tag == "Player")
            {
                anim.SetBool("IsWalking", true);
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
        anim.enabled = false;
        setRagdollState(true);
        Destroy(this.gameObject, 15);
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
