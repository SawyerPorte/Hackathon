using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WinCon : MonoBehaviour
{
    [SerializeField] Camera cam;
    public float speed;
    Vector3 target;
    public bool zoom;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        zoom = false;
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
            print("you win");
            zoom = true;
            target = transform.position;
            StartCoroutine(EndLevel());
        }
    }

    IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(3);
        print("switch level");
    }


    
}
