using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    public enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        DIE
    }

    private const int MONSTER_SCORE = 50;
    private const int MONSTER_HIT_DAMAGE = 10;
    private const int MONSTER_MAX_HP = 100;
    public State state = State.IDLE;
    public float traceDist = 10.0f;
    public float attackDist = 2.0f;
    public bool isDie = false;

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator anim;

    // Animator parameter의 Hash값 추출
    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");

    private int hp = MONSTER_MAX_HP;

    // 혈흔효과 prefab
    private GameObject bloodEffect;

    private void Awake()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        anim = GetComponent<Animator>();

        // load bloodEffect prefab
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");
    }
     void Update()
    {
        if (agent.remainingDistance >= 2.0f)
        {
            Vector3 direction = agent.desiredVelocity;
            Quaternion rot = Quaternion.LookRotation(direction);
            monsterTr.rotation = Quaternion.Slerp(monsterTr.rotation,rot,Time.deltaTime*10.0f);
        }
    } 

    // 스크립트가 활성화될때마다 호출되는 함수
    private void OnEnable()
    {
        // 이벤트 발생 시 수행할 함수 연결
        PlayerCtrl.OnPlayerDie += OnPlayerDie;

        // 몬스터의 상태를 체크하는 코루틴 호출
        StartCoroutine(CheckMonsterState());
        // 상태에 따라 몬스터의 행동을 수행하는 코루틴 호출
        StartCoroutine(MonsterAction());
    }

    // 스크립트가 비활성화될때마다 호출되는 함수
    private void OnDisable()
    {
        // 연결된 함수 해제
        PlayerCtrl.OnPlayerDie -= OnPlayerDie;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            // 충돌한 총알을 삭제
            Destroy(collision.gameObject);
            anim.SetTrigger(hashHit);
        }
    }

    public void OnDamage(Vector3 pos, Vector3 normal)
    {
        anim.SetTrigger(hashHit);
        // 총알 충돌 지점의 법선벡터
        Quaternion rot = Quaternion.LookRotation(normal);
        ShowBloodEffect(pos, rot);

        hp -= MONSTER_HIT_DAMAGE;
        if (hp <= 0)
        {
            state = State.DIE;
            // 몬스터가 사망했을 때 주어진 점수를 추가
            GameManager.instance.DisplayScore(MONSTER_SCORE);
        }
    }


    private void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTr);
        Destroy(blood, 1.0f);
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    agent.isStopped = true;
                    // Animator의 IsTrace변수를 false로 설정
                    anim.SetBool(hashTrace, false);
                    break;
                case State.ATTACK:
                    anim.SetBool(hashAttack, true);
                    break;
                case State.TRACE:
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    anim.SetBool(hashAttack, false);
                    break;
                case State.DIE:
                    isDie = true;
                    agent.isStopped = true;
                    anim.SetTrigger(hashDie);
                    // 몬스터의 Collider component 비활성화
                    GetComponent<CapsuleCollider>().enabled = false;
                    // 몬스터 손에 있는 PUNCH 비활성화
                    SphereCollider[] sc = GetComponentsInChildren<SphereCollider>();
                    foreach (var item in sc)
                    {
                        item.enabled = false;
                    }

                    // 일정 시간 대기 후 오브젝트 풀링으로 환원
                    yield return new WaitForSeconds(3.0f);
                    // 사망 후 다시 사용할 때를 위해 hp 값 초기화
                    hp = 100;
                    isDie = false;
                    // 몬스터의 Collider 컴포넌트 활성화
                    GetComponent<CapsuleCollider>().enabled = true;
                    // punch 활성화
                    foreach (var item in sc)
                    {
                        item.enabled = true;
                    }
                    // 몬스터 상태를 Idle 로 변환
                    this.state = State.IDLE;
                    // 몬스터를 비활성화
                    this.gameObject.SetActive(false);
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            // 0.3초 중지 하는 동안 양보
            yield return new WaitForSeconds(0.3f);

            if (state == State.DIE) yield break;

            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            if (distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if (distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }
    }

    private void OnPlayerDie()
    {
        StopAllCoroutines();

        agent.isStopped = true;
        anim.SetFloat(hashSpeed, Random.Range(0.8f, 1.2f));
        anim.SetTrigger(hashPlayerDie);
    }
}