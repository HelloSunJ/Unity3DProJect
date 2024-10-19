using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;

    // Start is called before the first
    // frame update


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision coll)
    {   if (coll.collider.CompareTag("BULLET"))
        {
            ContactPoint cp = coll.GetContact(0);
            Quaternion rot = Quaternion.LookRotation(-cp.normal);

            GameObject spark = Instantiate(sparkEffect, cp.point, rot);
            Destroy(spark, 0.5f);

            //Instantiate(sparkEffect, coll.transform.position, Quaternion.identity);
            Destroy(coll.gameObject);
        }
    }
}
