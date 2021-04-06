using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            var enemy = collision.gameObject.GetComponent<BaseEnemyController>();
            enemy.TakeDamage(5, collision);
        }


        Destroy(this.gameObject);
    }
}
