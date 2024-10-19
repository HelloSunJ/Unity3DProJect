using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
   // [SerializeField]

    private Animation anim;
    private Transform tr;
    public float moveSpeed = 8.0f;
    public float turnSpeed = 80.0f;

    private readonly float initHp = 100.0f;
    private float currHp = 80f;
    private Image haBar;

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    IEnumerator Start()
    {
        haBar = GameObject.FindGameObjectWithTag("HP_BAR")?.GetComponent<Image>();
        currHp = initHp;
        DisplayHealth();

        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();

        anim.Play("Idle");

        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.3f);
        turnSpeed = 80.0f;

    }

    // Update is called once per frame
    void Update()
    {
       // tr.Translate(Vector3.forward* Time.deltaTime * 10);
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");

       // Debug.Log("h=" + h);
       // Debug.Log("v=" + v);

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        
        tr.Translate(moveDir.normalized * Time.deltaTime  * moveSpeed);

        tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);

        PlayerAnim(h, v);
    }

    void PlayerAnim(float h, float v)
    {
        if (v >= 0.1f)
        {
            anim.CrossFade("RunF", 0.25f);
        }
        else if (v <= -0.1f) 
        {
            anim.CrossFade("RunB", 0.25f);
        }
         else if (h >= 0.1f) 
        {
            anim.CrossFade("RunR", 0.25f);
        }
        else if (h <= -0.1f) 
        {
            anim.CrossFade("RunL", 0.25f);
        }
        else
        {
            anim.CrossFade("Idle", 0.25f);
        }

    }

    void OnTriggerEnter(Collider coll)
    {
       if (currHp >= 0.0f && coll.CompareTag("PUNCH"))
        {
            currHp -= 10.0f;
            DisplayHealth();

            Debug.Log($"Player Hp = {currHp / initHp}");

            if (currHp <= 0.0f)
            {
                PlayerDie();
            }

          
        }
    }

    

    void PlayerDie()
    {
        Debug.Log("Player Die...!");

        //GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
        //foreach (GameObject monster in monsters)
        //{
        //    monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}
        OnPlayerDie();
        GameObject.Find("GameManager").GetComponent<GameManager>().IsGameOver = true;
            
    }

    void DisplayHealth()
    {
        haBar.fillAmount = currHp / initHp;
    }

    

}