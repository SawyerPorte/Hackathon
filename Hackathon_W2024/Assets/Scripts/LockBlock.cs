using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Requirements:
/// A locked gate that requires a specific type of block.
/// 
/// Opens after the appropriate block is brought to the gate
/// and the player interacts.
/// 
/// Player can access the submitted item again after the object's
/// animation happens.
/// 
/// </summary>
public class LockBlock : Blocks
{
    [SerializeField, Header("Required block type")]
    public GameObject requiredBlock;

    private bool isOccupied;    // Flag if lock has submission or not
    private bool isInteractable; // Flag if lock is interactable

    private void Start()
    {
        foreach (Transform childTransform in gameObject.transform)
        {
            GameObject childObject = childTransform.gameObject;
            if (childObject.name.Equals("RequirementSlot") && childObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
            {
                spriteRenderer.color = Color.white;
            }
        }

        // Assign the sprite renderer accordingly to the required block
        UpdateVisuals();
    }

    /// <summary>
    /// Call after item added or removed
    /// </summary>
    private void UpdateVisuals()
    {
        foreach (Transform childTransform in gameObject.transform)
        {
            GameObject childObject = childTransform.gameObject;
            if (childObject.name.Equals("SubmissionSlot") && childObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
            {
                if (isOccupied)
                {
                    //Render sprite accordingly to submission object
                    spriteRenderer.sprite = requiredBlock.GetComponent<SpriteRenderer>().sprite;
                }
                else
                {
                    spriteRenderer.color = Color.white;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isOccupied)
        {
            if (collision.gameObject.GetComponent<Player>().GetPickedUpObject() != null)
            {
                if (collision.gameObject.GetComponent<Player>().GetPickedUpObject().Equals(requiredBlock))
                {
                    // Show that it is correct
                    Debug.Log("correct");
                    UpdateVisuals();
                }
                else
                {
                    // Show that it is incorrect
                    Debug.Log("incorrect");
                    UpdateVisuals();
                }
            }
        }
    }
}
