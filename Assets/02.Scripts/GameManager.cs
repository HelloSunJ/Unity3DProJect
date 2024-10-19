using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //몬스터가 출현할 위치를 저장할 List
    public List<Transform> points = new List<Transform>();
    public List<GameObject> monsterPool = new List<GameObject>();
    public int maxMonsters = 10;
    public GameObject monster;
    public float createTime = 3.0f;
    private bool isGameOver;

    public bool IsGameOver
    {
        get{return isGameOver; }
        set{
            isGameOver = value;
            if(isGameOver)
            {
                CancelInvoke("CreateMonster");
            }
        }
    }

    public static GameManager instance = null;

    public TMP_Text scoreText;
    private int totScore = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if ( instance != this)
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        CreateMonsterPool();

        //SpwanPointGroup의 Transform 추출
        Transform spwanPointGroup = GameObject.Find("SpwanPointGroup")?.transform;
        // SpwanPointGroup 하위에 있는 모든 차일드 오브젝트의 Transform 추출
        //spwanPointGroup?.GetComponentsInChildren<Transform>(points);

        foreach (Transform point in spwanPointGroup)
        {
            points.Add(point);
        }
            InvokeRepeating("CreateMonster", 2.0f, createTime);
            totScore = PlayerPrefs.GetInt("Tot_SCORE", 0);
            DisplayScore(0);
    }

    void CreateMonster()
    {
        int idx = Random.Range(0, points.Count);
        //Instantiate(monster, points[idx].position, points[idx].rotation);
        GameObject _monster = GetMonsterInPool();
        _monster?.transform.SetPositionAndRotation(points[idx].position, points[idx].rotation);
        _monster.SetActive(true);
    }

    
    void CreateMonsterPool()
    {
        for(int i=0; i < maxMonsters; i++)
        {
            var _monster = Instantiate<GameObject>(monster);
            _monster.name = $"Monster_{i:00}";
            _monster.SetActive(false);

            monsterPool.Add(_monster);
        }
    }

    public GameObject GetMonsterInPool()
    {
        foreach(var _monster in monsterPool)
        {
            if (_monster.activeSelf == false)
            {
                return _monster;
            }
        }
        return null;
    }

    public void DisplayScore(int score)
    {
        totScore += score;
        if(scoreText != null)
        {scoreText.text =$"<color=#00ff00>SCORE : </color> <color=#ff0000>{totScore:#,##0}<color>";}
        PlayerPrefs.SetInt("TOT_SCORE", totScore);
        

    }
}
