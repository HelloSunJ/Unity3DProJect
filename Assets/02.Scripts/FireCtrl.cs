using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class FireCtrl : MonoBehaviour

{
    public GameObject bullet;
    public Transform firepos;
    public AudioClip fireSfx;
    private new AudioSource audio;
    public MeshRenderer muzzleFlash;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        muzzleFlash = firepos.GetComponentInChildren<MeshRenderer>();
        muzzleFlash.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(firepos.position, firepos.forward * 10.0f, Color.green);
        if (Input.GetMouseButtonDown(0))
        {
            Fire();

            if (Physics.Raycast(firepos.position,firepos.forward,out hit,10.0f, 1 << 6))
            {
                Debug.Log($"Hit ={hit.transform.name}");
                hit.transform.GetComponent<MonsterCtrl>()?.OnDamage(hit.point, hit.normal);
            }

        }
    }

    void Fire()
    {
        Instantiate(bullet, firepos.position, firepos.rotation);
        audio.PlayOneShot(fireSfx, 1.0f);

        StartCoroutine(ShowMuzzleFlash());

    }

    IEnumerator ShowMuzzleFlash()
    {
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        muzzleFlash.material.mainTextureOffset = offset;



        float angle = Random.Range(0, 360);
        muzzleFlash.transform.localRotation = Quaternion.Euler(0, 0, angle);

        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        muzzleFlash.enabled = true;

        yield return new WaitForSeconds(0.2f);

        muzzleFlash.enabled = false;
             
    }
}
