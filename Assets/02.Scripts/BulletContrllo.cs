using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletContrllo : MonoBehaviour
{
        public float damage = 20.0f;
        public float force = 1500.0f;

         private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * force);
        
         

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
