using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject expEffecf;

    public Texture[] textures;
    private new MeshRenderer renderer;

    public float radius = 10.0f;

    private Transform tr;
    private Rigidbody rb;

    private int hitcount = 0;
    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();

        //하위에 있는  meshrenderer 추출
        renderer = GetComponentInChildren<MeshRenderer>();
        int idx = Random.Range(0, textures.Length);

        renderer.material.mainTexture = textures[idx];
    }

    // Update is called once per frame
    void Update()
    {
        
    }


     void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("BULLET"))
        {
            if(++hitcount ==3)
            {
                ExpBarrel();
            }
        }
    }

    void ExpBarrel()
    {
        GameObject exp = Instantiate(expEffecf, tr.position, Quaternion.identity);

        Destroy(exp, 5.0f);

        //rb.mass = 1.0f;
        //rb.AddForce(Vector3.up * 1500.0f);

        IndirectDamage(tr.position);

        Destroy(gameObject, 3.0f);
    }

    void IndirectDamage(Vector3 pos)
    {
        Collider[] colls = Physics.OverlapSphere(pos, radius, 1 << 3);

        foreach(var coll in colls)
        {
            if(coll != null)
            rb = coll.GetComponent<Rigidbody>();
            rb.mass = 1.0f;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddExplosionForce(1500.0f, pos, radius, 1200.0f);

        }
    }

}

