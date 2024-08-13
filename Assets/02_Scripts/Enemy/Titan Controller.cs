using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

public class TitanController : MonoBehaviour
{
    // 열거형 변수를 정의
    public enum State { IDLE, TRACE, ATTACK, DIE };

    // 몬스터의 상태를 저장하는 변수
    public State state = State.IDLE;
    
 // HP 바를 연결하기 위한 Slider 변수
    public Slider hpSlider;
 

    // 공격사정거리
    [SerializeField] private float attackDist =  3.0f;
    // 추적사정거리
    [SerializeField] private float traceDist = 20.0f;
    [SerializeField] private float BossinitHp = 100.0f;
    [SerializeField] private float BosscurrHp = 100.0f;

    private Transform playerTr;
    // 몬스터의 Transform
    private Transform monsterTr;
    private NavMeshAgent agent;
    private Animator anim;

    private bool isDie = false;

    // Animator View의 Parameter Hash값을 미리 추출
    private readonly int hashIsTrace = Animator.StringToHash("IsTrace");
    private readonly int hashIsAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashDie = Animator.StringToHash("Die");

    void OnEnable()
    {
        // 이벤트 리스너 등록(Event Subscription)

    }

    void OnDisable()
    {
        // 이벤트 리스너 해제
      
    }

    void Start()
    {
        monsterTr = GetComponent<Transform>(); // monsterTr = transform;
        playerTr = GameObject.FindGameObjectWithTag("PLAYER")?.GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());

        BosscurrHp = BossinitHp;
        hpSlider.maxValue = BossinitHp;
        hpSlider.value = BosscurrHp;
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            // 몬스터의 상태값에 따라 분기처리
            switch (state)
            {
                case State.IDLE:
                    agent.isStopped = true;
                    anim.SetBool(hashIsTrace, false);
                    anim.SetBool(hashIsAttack, false);

             
                
                    break;

                case State.TRACE:
                    // 추적로직
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    anim.SetBool(hashIsAttack, false);
                    anim.SetBool(hashIsTrace, true);
                    break;

                case State.ATTACK:
                    anim.SetBool(hashIsAttack, true);
                    break;

                case State.DIE:
                    anim.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    agent.isStopped = true;
                    isDie = true;
                    break;
                       
            }
                yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator CheckMonsterState()
    {
        while (isDie == false)
        {
            if (state == State.DIE) yield break;

            // 두 위치간의 거리를 측정
            float distance = Vector3.Distance(monsterTr.position, playerTr.position);
            state = State.IDLE;

            // 추적 사정거리 이내일 경우
            if (distance <= traceDist)
            {
                state = State.TRACE;
            }

          

            // 공격 사정거리 이내일 경우
            if (distance <= attackDist)
            {
                state = State.ATTACK;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
  
    
    void OnCollisionEnter(Collision coll)
    {
       if (BosscurrHp > 0.0f && coll.gameObject.CompareTag("PUNCH"))
        {
            BosscurrHp -= 10.0f;

    

            if (BosscurrHp <= 0.0f)
            
            {
                state = State.DIE;
            }
        }
    }


    public void OnDamage()
    {
        // Hit Reaction...
        anim.SetTrigger(hashHit);

        BosscurrHp -= 20;
        if (BosscurrHp <= 0)
        {
            state = State.DIE;
        }
    }

    public void TakeDamage(float damage)
    {
        BosscurrHp -= damage;
        BosscurrHp = Mathf.Clamp(BosscurrHp , 0 , BossinitHp);
        hpSlider.value = BosscurrHp;
         if (BosscurrHp <= 0)
        {
            state = State.DIE;
        }
    }
        public void Heal(float amount)
    {
        BosscurrHp += amount;

        // 체력이 최대 체력(initHp)을 초과하지 않도록 설정합니다.
        BosscurrHp = Mathf.Clamp(BosscurrHp, 0, BossinitHp);

        // HP 바를 업데이트합니다.
        hpSlider.value = BosscurrHp;
        
    }

    void Update()
{
    // 테스트용으로 체력을 10씩 감소시킵니다.
    if (Input.GetKeyDown(KeyCode.Space))
    {
        TakeDamage(10);
    }

    // 테스트용으로 체력을 5씩 회복시킵니다.
    if (Input.GetKeyDown(KeyCode.H))
    {
        Heal(5);
    }
     if (BosscurrHp <= 0)
        {
            state = State.DIE;
        }
    }
}


//     public void YouWin()
//     {
//         // 공격 애니메이션 중지
//         // Dance
//         StopAllCoroutines();
//         agent.isStopped = true;

//         anim.SetTrigger("PlayerDie");
//     } 
    
// }