using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] KeyCode leftKey, rightKey, jumpKey, pickUpKey;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float maxJumpHeight = 10f;
    [SerializeField] float chargeRate = 2f;

    private ScreenShake camShake;
    private Rigidbody2D rb;
    private bool isGrounded;
    private float jumpCharge;
    private bool shakeCam = false;

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
        if(isGrounded)
            MoveLogic();
    }

    private void MoveLogic()
    {
        // Player movement
        float moveInput = 0f;
        if (Input.GetKey(leftKey))
        {
            moveInput = -1f;
        }
        else if (Input.GetKey(rightKey))
        {
            moveInput = 1f;
        }
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }
    private void JumpLogic()
    {
        // Check if player is grounded
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.8f, LayerMask.GetMask("Ground"));

        if (isGrounded && rb.velocity.y < 0)
        {
            // Check if cameraShake is not null to avoid errors
            if (camShake != null && shakeCam)
            {
                // Trigger camera shake
                shakeCam = false;
                float intensity = Mathf.Clamp01(Mathf.Abs(rb.velocity.y) / 10f);
                print("intensity: " + intensity);
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
            rb.velocity = new Vector2(rb.velocity.x, jumpCharge);
            shakeCam = true;
            jumpCharge = 0f;
        }
    }
}
