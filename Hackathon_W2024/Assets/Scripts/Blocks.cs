using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BlockType
{
    Normal,
    Bouncy,
    Sticky,
    Moving,
    Heavy,
    Light,
    Dump
    // Add more block types as needed
}
public class Blocks : MonoBehaviour
{
    [SerializeField] BlockType blockType;

    [Header("Block specific vars")]
    [SerializeField] float bounceForce = 5;
    [SerializeField] float stickDistance = 1f;
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] LayerMask stickLayerMask;

    private Rigidbody2D rb;
    private Collider2D closestCollider;
    Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        switch (blockType)
        {
            case BlockType.Normal:
                break;
            case BlockType.Sticky:
                StickyLogicV2();
                break;
            case BlockType.Bouncy:
                break;
            case BlockType.Moving:
                MovingLogic();
                break;
            case BlockType.Heavy:
                break;
            case BlockType.Light:
                break;
            case BlockType.Dump:
                break;
            default:
                Debug.Log("blockType Error");
                break;
        }
    }

    // return block type
    public BlockType GetBlockType()
    {
        return this.blockType;
    }

    private void StickyLogicV2()
    {
        foreach (Vector2 direction in directions)
        {
            //RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, stickDistance, stickLayerMask);
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, .001f, direction, stickDistance, stickLayerMask);
            if (hits.Length > 0)
            {
                // Find the closest hit point
                Vector2 closestPoint = hits[0].point;
                foreach (var hit in hits)
                {
                    if (Vector2.Distance(transform.position, hit.point) < Vector2.Distance(transform.position, closestPoint))
                    {
                        closestPoint = hit.point;
                    }
                }

                // Calculate the position to move the player to, considering the object's width
                Vector2 newPosition = closestPoint;

                // Move the player to the adjusted position
                transform.position = newPosition;
                print("new block pos: " + newPosition);
                rb.constraints = RigidbodyConstraints2D.FreezePosition;
                rb.freezeRotation = true;
                break; // Stop iterating if any circlecast hits an object
            }
        }
    }
    private void StickyLogic()
    {
        // Find all colliders within the stickDistance range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, stickDistance, stickLayerMask);

        // Initialize variables to keep track of the closest collider
        float closestDistance = Mathf.Infinity;
        closestCollider = null;

        // Iterate through all colliders found
        foreach (Collider2D collider in colliders)
        {
            // Calculate the distance between the object and the collider
            float distance = Vector2.Distance(transform.position, collider.transform.position);

            // Check if this collider is closer than the previously closest one
            if (distance < closestDistance)
            {
                // Update the closest collider and distance
                closestCollider = collider;
                closestDistance = distance;
            }
        }

        // Check if a closest collider was found
        if (closestCollider != null)
        {
            // Perform actions with the closest collider (e.g., stick to it)
            StickToCollider(closestCollider);
        }
        else
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePosition;
            rb.freezeRotation = true;
        }
    }

    private void MovingLogic()
    {
        /*if (!facingRight)
        {
            moveSpeed *= -1;
        }*/
        // move block position forward at a given speed
        this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x + this.moveSpeed, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
        //Debug.Log("moving direction: " + this.gameObject.transform.position);
    }
    
    private void StickToCollider(Collider2D collider)
    {
        // Get the closest point on the collider's surface to the object's current position
        Vector2 closestPoint = collider.ClosestPoint(transform.position);

        // Calculate the direction from the object's current position to the closest point on the collider's surface
        Vector2 directionToClosestPoint = (Vector2)transform.position - closestPoint;

        // Calculate the desired position outside the collider
        Vector2 desiredPosition = closestPoint + directionToClosestPoint.normalized * (stickDistance / 2);

        // Set the object's position to the desired position outside the collider
        transform.position = desiredPosition;


        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        rb.freezeRotation = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BouncyBlock"))
        {
            // Calculate the direction of the bounce (normal of the collision)
            Vector2 bounceDirection = collision.contacts[0].normal;
            
            Vector2 newBounceForce = bounceDirection * collision.gameObject.GetComponent<Blocks>().ReturnBounceForce() * rb.mass;
            print("block BOUNCE: " + newBounceForce);
            // Apply a force in the bounce direction to the object
            rb.AddForce(newBounceForce, ForceMode2D.Impulse);
        }
    }
    public float ReturnBounceForce()
    {
        return bounceForce;
    }
}