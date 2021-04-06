using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyController : MonoBehaviour
{
    public int Health = 10;
    public Transform Player;

    Collider col;
    Animator anim;
    Rigidbody rb;

    Vector3 lastCollisionPoint;

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
        if (CheckForPlayer())
        {
            MoveToPlayer();
        }

        if (Health <= 0)
        {
            Die();

            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(1, lastCollisionPoint, 1, 0, ForceMode.Impulse);
            }
        }
    }

    void MoveToPlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(Player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 15 * Time.deltaTime);
    }

    bool CheckForPlayer()
    {
        RaycastHit hit;
        if(Physics.Linecast(transform.position, Player.transform.position, out hit))
        {
            if(hit.transform == Player.transform)
            {
                return true;
            }
        }

        return false;
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
