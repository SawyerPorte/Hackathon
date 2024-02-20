using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] KeyCode leftKey, rightKey, jumpKey, pickUpKey;
    [Header("Movement vars")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Jump vars")]
    [SerializeField] LayerMask groundLayers;
    [SerializeField] float maxJumpHeight = 10f;
    [SerializeField] float minJumpHeight = 3f;
    [SerializeField] float chargeRate = 2f;

    private bool isGrounded;
    private float jumpCharge;
    private bool shakeCam = false;

    [Header("Block vars")]
    [SerializeField] LayerMask pickUpLayer;
    [SerializeField] LayerMask dropLayer;
    [SerializeField] float pickUpDistance = 2f;
    [Tooltip("how much in front of the player the block will be placed")] [SerializeField] float placeInFrontDistance = 1f;

    private GameObject pickedUpObject; // The object currently picked up
    private bool isHoldingObject = false;
    GameObject closestObject;

    private ScreenShake camShake;
    private Rigidbody2D rb;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        camShake = Camera.main.GetComponent<ScreenShake>();
    }

    // Update is called once per frame
    void Update()
    {
        JumpLogic();
        if (!shakeCam)
        {
            MoveLogic();
        }

        PickUpInputLogic();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BouncyBlock"))
        {
            // Calculate the direction of the bounce (normal of the collision)
            Vector2 bounceDirection = collision.contacts[0].normal;
            print("bounceDirection: " + bounceDirection);

            // Apply a force in the bounce direction to the object
            rb.AddForce(bounceDirection * moveSpeed * 2, ForceMode2D.Impulse);
        }
    }

    private void PickUpInputLogic()
    {
        if (Input.GetKeyDown(pickUpKey))
        {
            if (!isHoldingObject)
                PickUpObject();
            else
                DropObject();
        }

        if (isHoldingObject)
        {
            ReturnClosestDropPoint();
            if (closestObject == null)
                pickedUpObject.transform.position = transform.position + Vector3.up * 1.5f;
            else
            {
                pickedUpObject.transform.position = closestObject.transform.position;
                pickedUpObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
                pickedUpObject.GetComponent<Rigidbody2D>().freezeRotation = true;
            }
                
        }
    }

    private void PickUpObject()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickUpDistance, pickUpLayer);

        if (colliders.Length > 0)
        {
            GameObject closestObject = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider2D collider in colliders)
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestObject = collider.gameObject;
                    closestDistance = distance;
                }
            }

            // Move the closest object above the player
            if (closestObject != null)
            {
                pickedUpObject = closestObject;
                isHoldingObject = true;
                pickedUpObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                pickedUpObject.GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePosition;
                pickedUpObject.transform.position = transform.position + Vector3.up * 1.5f;
            }
        }
    }
    private GameObject ReturnClosestDropPoint()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickUpDistance, dropLayer);
        closestObject = null;
        if (colliders.Length > 0)
        {
            // Find the closest object on the specified layer
            
            float closestDistance = Mathf.Infinity;

            foreach (Collider2D collider in colliders)
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestObject = collider.gameObject;
                    closestDistance = distance;
                }
            }
        }
        return closestObject;
    }
    private void DropObject()
    {
        if (pickedUpObject != null)
        {
            // Check for nearby objects on the specified layer
            //Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickUpDistance, dropLayer);
            ReturnClosestDropPoint();
            if (closestObject != null)
            {
                /*
                // Find the closest object on the specified layer
                GameObject closestObject = null;
                float closestDistance = Mathf.Infinity;

                foreach (Collider2D collider in colliders)
                {
                    float distance = Vector2.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestObject = collider.gameObject;
                        closestDistance = distance;
                    }
                }
                */
                // Place the picked up object at the position of the closest object
                if (closestObject != null)
                {
                    pickedUpObject.transform.position = closestObject.transform.position;
                    pickedUpObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
                    pickedUpObject.GetComponent<Rigidbody2D>().freezeRotation = true;
                    //pickedUpObject.GetComponent<Rigidbody2D>().gravityScale = 1;
                }
            }
            else
            {
                // If no nearby objects on the specified layer, drop the object at the player's position
                pickedUpObject.transform.position = transform.position + (transform.right * (transform.localScale.x > 0 ? 1f : -1f)) * placeInFrontDistance;
                pickedUpObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            }

            isHoldingObject = false;
            pickedUpObject = null;
        }
    }

    private void MoveLogic()
    {
        // Player movement
        float moveInput = 0f;
        if (Input.GetKey(leftKey))
        {
            moveInput = -1f;
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (Input.GetKey(rightKey))
        {
            moveInput = 1f;
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }
    private void JumpLogic()
    {
        // Check if player is grounded
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.8f, groundLayers);

        if (isGrounded && rb.velocity.y < 0)
        {
            // Check if cameraShake is not null to avoid errors
            if (camShake != null && shakeCam)
            {
                // Trigger camera shake
                shakeCam = false;
                float intensity = Mathf.Clamp01(Mathf.Abs(rb.velocity.y) / 10f);
                //print("intensity: " + intensity);
                camShake.ShakeCamera(intensity);
            }
        }

        // Charging jump
        if (Input.GetKey(jumpKey) && isGrounded)
        {
            jumpCharge += chargeRate * Time.deltaTime;
            jumpCharge = Mathf.Clamp(jumpCharge, 0f, maxJumpHeight);
        }

        // Jumping
        if (Input.GetKeyUp(jumpKey) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpCharge + minJumpHeight);
            shakeCam = true;
            jumpCharge = 0f;
        }
    }
}
