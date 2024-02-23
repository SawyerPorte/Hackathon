using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private int jumpsLeft = 1;
    private bool facingRight;

    [Header("Block vars")]
    [SerializeField] GameObject hiddenBoxCollider;
    [SerializeField] LayerMask pickUpLayer;
    [SerializeField] LayerMask dropLayer;
    [SerializeField] float pickUpDistance = 2f;
    [SerializeField] Animator animator;
    [SerializeField] float holdHeight = .5f;
    [Tooltip("how much in front of the player the block will be placed")] [SerializeField] float placeInFrontDistance = 1f;

    private GameObject pickedUpObject; // The object currently picked up
    private bool isHoldingObject = false;
    GameObject closestObject;

    // Logic for Lock Block interaction
    public bool isNearLock = false;
    GameObject currentLockRef = null;

    private ScreenShake camShake;
    private Rigidbody2D rb;

    // Animator
    private Animator playerAnimator = null;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        camShake = Camera.main.GetComponent<ScreenShake>();

        //GameObject charModel = GameObject.Find("CharacterModel");
        playerAnimator = animator;

        if (playerAnimator != null)
        {
            // Set animator states
            playerAnimator.SetBool("isMoving", false);
            playerAnimator.SetBool("isJumping", false);
            playerAnimator.SetBool("isFalling", false);
        }
        else
        {
            // Try again by including inactive GameObjects.
            playerAnimator = GetComponentInChildren<Animator>(true);

            if (playerAnimator != null)
                // Set animator states
                playerAnimator.SetBool("isMoving", false);
                playerAnimator.SetBool("isJumping", false);
                playerAnimator.SetBool("isFalling", false);
        }

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
            rb.AddForce(bounceDirection * collision.gameObject.GetComponent<Blocks>().ReturnBounceForce(), ForceMode2D.Impulse);
        }

        if (collision.gameObject.CompareTag("LockBlock") && !isNearLock)
        {
            Debug.Log("stepping into range");
            currentLockRef = collision.gameObject;
            isNearLock = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("LockBlock") && isNearLock)
        {
            Debug.Log("stepping out of range");
            currentLockRef = null;
            isNearLock = false;
        }
    }

    private void PickUpInputLogic()
    {
        if (Input.GetKeyDown(pickUpKey))
        {
            if (!isHoldingObject)
            {
                if (!isNearLock)
                {
                    PickUpObject();
                }
                else
                {
                    // If player is in range of a lock block, take the item from the block
                    RecieveObject();
                }
            }
            else
            {
                if (!isNearLock)
                {
                    DropObject();
                }
                else
                {
                    // If player is in range of a lock block, check submission into block
                    SubmitObject();
                }
            }
        }

        if (isHoldingObject)
        {
            ReturnClosestDropPoint();
            if (closestObject == null)
            {
                if (pickedUpObject.transform.parent == null)
                {
                    pickedUpObject.transform.parent = this.gameObject.transform;
                    pickedUpObject.transform.position = transform.position + Vector3.up * holdHeight;

                }
                
            }
            else
            {
                pickedUpObject.transform.parent = null;
                pickedUpObject.transform.position = closestObject.transform.position;
                pickedUpObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
                pickedUpObject.GetComponent<Rigidbody2D>().freezeRotation = true;
            }
        }
    }

    private void PickUpObject()
    {
        // play pick up sound
        //SoundManager.Instance.PlayGameSound("PickUp");

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pickUpDistance, pickUpLayer);

        if (colliders.Length > 0)
        {
            GameObject closestObject = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider2D collider in colliders)
            {
                if(collider.gameObject.GetComponent<Blocks>().GetBlockType() != BlockType.Heavy)
                {
                    float distance = Vector2.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestObject = collider.gameObject;
                        closestDistance = distance;
                    }
                }
            }

            // Move the closest object above the player
            if (closestObject != null)
            {
                playerAnimator.SetTrigger("PickUpTrigger");
                playerAnimator.SetBool("PickUp", true);
                pickedUpObject = closestObject;
                isHoldingObject = true;
                pickedUpObject.GetComponent<Rigidbody2D>().gravityScale = 0;
                pickedUpObject.GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePosition;
                pickedUpObject.transform.position = transform.position + Vector3.up * holdHeight;
                pickedUpObject.GetComponent<BoxCollider2D>().isTrigger = true;
                pickedUpObject.GetComponent<Rigidbody2D>().isKinematic = true;
                hiddenBoxCollider.SetActive(true);
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
        // play pick up sound
        //SoundManager.Instance.PlayGameSound("PickUp");

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
                if (pickedUpObject.GetComponent<Blocks>().GetBlockType() == BlockType.Normal)
                {
                    if (!pickedUpObject.GetComponent<Blocks>().GetIsStuck())
                        pickedUpObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
                } else if (pickedUpObject.GetComponent<Blocks>().GetBlockType() == BlockType.Light)
                {
                    if (!pickedUpObject.GetComponent<Blocks>().GetIsStuck())
                        pickedUpObject.GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePosition;
                // Moving Block Behavior when dropped
                } else if (pickedUpObject.GetComponent<Blocks>().GetBlockType() == BlockType.Moving)
                {
                    pickedUpObject.GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePosition;
                    
                    // Send who last interacted with block
                    if (this.gameObject.CompareTag("Player1"))
                    {
                        pickedUpObject.GetComponent<Blocks>().Player1LastInteracted();
                    }
                    else if (this.gameObject.CompareTag("Player2"))
                    {
                        pickedUpObject.GetComponent<Blocks>().Player2LastInteracted();
                    }
                    else
                    {
                        Debug.Log("Player Tag Not Defined");
                    }
                    
                    // set direction the moving block should face
                    if (FacingRight())
                    {
                        pickedUpObject.GetComponent<Blocks>().facingRight = true;
                    }
                    else
                    {
                        pickedUpObject.GetComponent<Blocks>().facingRight = false;
                    }
                }
                pickedUpObject.GetComponent<Rigidbody2D>().freezeRotation = true;
                if(!pickedUpObject.GetComponent<Blocks>().GetIsStuck())
                    pickedUpObject.transform.position = transform.position + (transform.right * (transform.localScale.x > 0 ? 1f : -1f)) * placeInFrontDistance;
                pickedUpObject.GetComponent<Rigidbody2D>().gravityScale = 2.5f;
                pickedUpObject.transform.parent = null;
            }
            pickedUpObject.GetComponent<Rigidbody2D>().isKinematic = false;
            pickedUpObject.GetComponent<BoxCollider2D>().isTrigger = false;
            hiddenBoxCollider.SetActive(false);
            isHoldingObject = false;
            pickedUpObject = null;
        }
        playerAnimator.SetBool("PickUp", false);
    }

    /// <summary>
    /// Function meant to remove the current object the player is holding and
    /// place it into lock block
    /// </summary>
    public void SubmitObject()
    {
        // check if there is a referenced object, if the lock is interactable, and is not occupied with a block
        if (currentLockRef)
        {
            if (currentLockRef.GetComponent<LockBlock>().RecieveSubmission(pickedUpObject))
            {
                // Visually remove the object
                Destroy(pickedUpObject);
                pickedUpObject = null;

                isHoldingObject = false;
            }
            else
            {
                // Call drop object instead
                DropObject();
            }
        }
        else
        {
            Debug.LogError("null reference to lock block");
        }
    }

    /// <summary>
    /// Function meant to remove the current object the player is holding and
    /// place it into lock block
    /// </summary>
    public void RecieveObject()
    {
        // check if there is a referenced object, if the lock is interactable, and is occupied with a block
        if (currentLockRef)
        {
            // check if object is null
            if (currentLockRef.GetComponent<LockBlock>().ReturnSubmission())
            {
                //TODO: Needs better way to give player block type back
                pickedUpObject = Instantiate(currentLockRef.GetComponent<LockBlock>().ReturnSubmission());
                pickedUpObject.SetActive(true);

                isHoldingObject = true;
            }
        }
        else
        {
            Debug.LogError("null reference to lock block");
        }
    }

    private void MoveLogic()
    {
        // Player movement
        float moveInput = 0f;

        if (Input.GetKey(leftKey) || Input.GetKey(rightKey)) {
            // Change animation state
            
            playerAnimator.SetBool("isMoving", true);
            
           
            if (Input.GetKey(leftKey))
            {
                moveInput = -1f;
                transform.localScale = new Vector3(-1f, 1f, 1f);
                facingRight = false;
            }
            else if (Input.GetKey(rightKey))
            {
                moveInput = 1f;
                transform.localScale = new Vector3(1f, 1f, 1f);
                facingRight = true;
            }
        }
        else
        {
            playerAnimator.SetBool("isMoving", false);
        }

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    private void JumpLogic()
    {
        // Check if player is grounded
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down,.8f, groundLayers);
        Debug.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - .8f), Color.red);

        if(rb.velocity.y < 0)
        {
            playerAnimator.SetBool("isFalling", true);
        }

        if (isGrounded)
        {
            // Set animation state
            //playerAnimator.SetBool("isFalling", true);
            playerAnimator.SetBool("isFalling", false);
            
            print("set falling");


            if(rb.velocity.y <= 0)
            {
                playerAnimator.SetBool("Grounded", true);
                jumpsLeft = 1;
            }
            
            if(rb.velocity.y < 0)
            {
                // Check if cameraShake is not null to avoid errors
                if (camShake != null && shakeCam)
                {
                    // Trigger camera shake
                    shakeCam = false;
                    float intensity = Mathf.Clamp01(Mathf.Abs(rb.velocity.y) / 10f);
                    camShake.ShakeCamera(intensity);
                }
            }
        }

        // Charging jump
        if (Input.GetKey(jumpKey) && jumpsLeft > 0)
        {
            jumpCharge += chargeRate * Time.deltaTime;
            jumpCharge = Mathf.Clamp(jumpCharge, 0f, maxJumpHeight);
        }

        // Jumping
        if (Input.GetKeyUp(jumpKey) && jumpsLeft > 0)
        {
            // Trigger jump sound
            //SoundManager.Instance.PlayGameSound("Jump");

            // Trigger jump animation
           
            if(rb.velocity.y == 0)
            {
                playerAnimator.SetTrigger("isJumpingTrigger");
            }
            

            rb.velocity = new Vector2(rb.velocity.x, jumpCharge + minJumpHeight);
            playerAnimator.SetBool("Grounded", false);
            shakeCam = true;
            jumpCharge = 0f;
            jumpsLeft--;
        }
    }

    /// <summary>
    /// Getter for picked up object
    /// </summary>
    /// <returns> If object is available, return a reference. Otherwise return null </returns>
    public GameObject GetPickedUpObject()
    {
        if (isHoldingObject)
        {
            return pickedUpObject;
        }
        else return null;
    }
    
    public bool FacingRight()
    {
        return facingRight;
    }

}
