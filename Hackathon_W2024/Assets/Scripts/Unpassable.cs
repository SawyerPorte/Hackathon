using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unpassable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, 7);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
