using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks : MonoBehaviour
{
    [SerializeField] float bounceForce = 5;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BouncyBlock"))
        {
            // Calculate the direction of the bounce (normal of the collision)
            Vector2 bounceDirection = collision.contacts[0].normal;
            print("bounceDirection: " + bounceDirection);

            // Apply a force in the bounce direction to the object
            rb.AddForce(bounceDirection * collision.gameObject.GetComponent<Blocks>().ReturnBounceForce(), ForceMode2D.Impulse);
        }
    }
    public float ReturnBounceForce()
    {
        return bounceForce;
    }
}
