using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    private GameObject bullet;
    private PlayerController player;
    [SerializeField] float impactForce = 5f;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("bullet"))
        {
            bullet = collision.gameObject;
            player = GetComponent<PlayerController>();
            player.CollisionScale();
            ApplyKnockBackToPlayer();
        }
    }

    private void ApplyKnockBackToPlayer()
    {
        Rigidbody _rb = bullet.GetComponent<Rigidbody>();
        Vector2 bulletVelocity = _rb.velocity.normalized;
        player.GetComponent<Rigidbody>().velocity = new Vector2(bulletVelocity.x  * player.GetCurrentScale()*impactForce,
           bulletVelocity.y * player.GetCurrentScale() * impactForce);
        

    }
}
