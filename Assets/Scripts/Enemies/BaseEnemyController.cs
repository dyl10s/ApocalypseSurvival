using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{
    public int Health = 10;

    Collider col;
    Animator anim;
    Rigidbody rb;

    Vector3 lastCollisionPoint;
    List<Rigidbody> RbsHitLast;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        setRagdollState(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Health <= 0)
        {
            Die();

            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(1, lastCollisionPoint, 1, 0, ForceMode.Impulse);
            }
        }
    }

    public void TakeDamage(int damage, Collision collision)
    {
        Health -= damage;
        lastCollisionPoint = collision.GetContact(0).point;
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
