using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCon : MonoBehaviour
{
    [SerializeField] Camera cam;
    public float speed;
    private int level;
    Vector3 target;
    public bool zoom;
    Animator endingAnimator;
    LevelManager levelManager;
    [SerializeField] SpriteRenderer playerOneSprite;
    [SerializeField] SpriteRenderer playerTwoSprite;
    [SerializeField] GameObject endingSprite;
    [SerializeField] GameObject playerTwo;
    [SerializeField] GameObject playerOne;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        zoom = false;
        endingAnimator = endingSprite.GetComponent<Animator>();
        GameObject managerChecker = GameObject.Find("Managers");
        if(managerChecker != null)
            levelManager = managerChecker.GetComponent<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (zoom)
        {

            cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(target.x, target.y, cam.transform.position.z), speed/1.1f *Time.deltaTime);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 3, speed * Time.deltaTime);
            
            //cam.transform.position = Vector3.Lerp(cam.transform.position, new Vector3(target.x, target.y, cam.transform.position.z), .0125f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Play win sound
            SoundManager.Instance.PlayUISound("Victory");

            //print("you win");
            zoom = true;
            
            endingSprite.SetActive(true);
            playerOneSprite.enabled = false;
            playerTwoSprite.enabled = false;
            if(playerTwo.transform.position.x > playerOne.transform.position.x)
            {
                endingSprite.GetComponent<SpriteRenderer>().flipX = true;
            }

            endingAnimator.SetTrigger("End");
            target = transform.position;
            StartCoroutine(EndLevel());
        }
    }

    IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(3);      
        //levelManager.SetCurrentLevel(PlayerPrefs.GetInt("LevelProgress"));
        levelManager.LoadNextLevel();
        
        //print("switch level");
    }


    
}
