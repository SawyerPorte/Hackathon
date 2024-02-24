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

    private bool isOccupied = false;    // Flag if lock has submission or not
    private bool isInteractable = true; // Flag if lock is interactable

    private void Start()
    {
        foreach (Transform childTransform in gameObject.transform)
        {
            GameObject childObject = childTransform.gameObject;
            if (childObject.name.Equals("SubmissionSlot") && childObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
            {
                //TODO: change to sprite instead of color later
                spriteRenderer.color = Color.white;
            }
            if (childObject.name.Equals("RequirementSlot") && childObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer sRenderer))
            {
                //TODO: change to sprite instead of color later
                sRenderer.sprite = requiredBlock.transform.GetComponent<SpriteRenderer>().sprite;
                //sRenderer.color = requiredBlock.transform.GetComponent<SpriteRenderer>().color;
            }
        }

        // Assign the sprite renderer accordingly to the required block
        UpdateVisuals();
    }

    /// <summary>
    /// Called by Player to give submission to lock block
    /// If equal to required block, take item
    /// If not equal, give negative feedback
    /// </summary>
    public bool RecieveSubmission(GameObject heldObject)
    {
        if (heldObject.GetComponent<Blocks>().GetBlockType() == requiredBlock.GetComponent<Blocks>().GetBlockType() 
            && !isOccupied && isInteractable)
        {
            //TODO: Give visual feedback from lock block to show submission was correct
            Debug.Log("lock block SUBMIT SUCCESS");
            gameObject.SetActive(false);

            // Update state
            isOccupied = true;
            UpdateVisuals();

            return true;
        }
        else
        {
            //TODO: Give visual feedback from lock block to show it was incorrect
            Debug.Log("lock block SUBMIT FAILURE");

            return false;
        }
    }

    /// <summary>
    /// Called by Player to get the block from the lock block
    /// If equal to required block, take item
    /// If not equal, give negative feedback
    /// </summary>
    public GameObject ReturnSubmission()
    {
        GameObject returnObject = null;
        if (isOccupied && isInteractable)
        {
            Debug.Log("lock block RETURN SUCCESS");
            returnObject = requiredBlock;

            // Update state
            isOccupied = false;
            UpdateVisuals();
        }
        else
        {
            Debug.Log("lock block RETURN SUCCESS");
        }
        return returnObject;
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

                    // temp color change
                    spriteRenderer.color = requiredBlock.GetComponent<SpriteRenderer>().color;
                }
                else
                {
                    spriteRenderer.color = Color.white;
                }
            }
        }
    }

    //TODO: Need to figure out why collider not working

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player") && !isInteractable)
    //    {
    //        Debug.Log("stepping into zone");

    //        collision.gameObject.GetComponent<Player>().isNearLock = true;
    //        isInteractable = true;
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player") && isInteractable)
    //    {
    //        Debug.Log("stepping out of zone");
    //        collision.gameObject.GetComponent<Player>().isNearLock = false;
    //        isInteractable = false;
    //    }
    //}

    //TODO: Animation for the lock to "back up" into the scene for players to pass
}
