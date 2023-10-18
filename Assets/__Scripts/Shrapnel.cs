using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shrapnel : MonoBehaviour {

   protected BoundsCheck bndCheck;
    // Start is called before the first frame update

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if this game object is off screen, destroy it
        if (bndCheck.offUp)
        {
            Destroy(gameObject);
        }
        if (bndCheck.offDown)
        {
            Destroy(gameObject);
        }
        if (bndCheck.offLeft)
        {
            Destroy(gameObject);
        }
        if (bndCheck.offRight)
        {
            Destroy(gameObject);
        }
    }
}
