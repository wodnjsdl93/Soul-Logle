// using UnityEngine;
// using UnityEngine.AI;
// using UnityEngine.UI;  // UI 요소를 사용하기 위해 추가

// public class TitanController : MonoBehaviour
// {
//     // 열거형 변수를 정의
//     public enum State { IDLE, TRACE, ATTACK, DIE };

//     // 몬스터의 상태를 저장하는 변수
//     public State state = State.IDLE;

//     // 공격사정거리
//     [SerializeField] private float attackDist = 3.0f;
//     // 추적사정거리
//     [SerializeField] private float traceDist = 20.0f;

//     // 주인공 캐릭터의 Transform 컴포넌트를 저장할 변수
//     private Transform playerTr;
//     // 몬스터의 Transform
//     private Transform monsterTr;
//     private NavMeshAgent agent;
//     private Animator anim;

//     private bool isDie = false;
//     private int hp = 250;
//     private int maxHp = 250;  // 최대 체력

//     // 체력바를 위한 UI 이미지
//     [SerializeField] private Image healthBar;  // 체력바 이미지 변수

//     // Animator View의 Parameter Hash값을 미리 추출
//     private readonly int hashIsTrace = Animator.StringToHash("IsTrace");
//     private readonly int hashIsAttack = Animator.StringToHash("IsAttack");
//     private readonly int hashHit = Animator.StringToHash("Hit");
//     private readonly int hashDie = Animator.StringToHash("Die");

//     void Start()
//     {
//         monsterTr = GetComponent<Transform>(); // monsterTr = transform;
//         playerTr = GameObject.FindGameObjectWithTag("PLAYER")?.GetComponent<Transform>();
//         agent = GetComponent<NavMeshAgent>();
//         anim = GetComponent<Animator>();

//         StartCoroutine(CheckMonsterState());
//         StartCoroutine(MonsterAction());

//         // 초기 체력바 설정
//         UpdateHealthBar();
//     }

//     IEnumerator MonsterAction()
//     {
//         while (!isDie)
//         {
//             switch (state)
//             {
//                 case State.IDLE:
//                     agent.isStopped = true;
//                     anim.SetBool(hashIsTrace, false);
//                     break;

//                 case State.TRACE:
//                     agent.SetDestination(playerTr.position);
//                     agent.isStopped = false;
//                     anim.SetBool(hashIsAttack, false);
//                     anim.SetBool(hashIsTrace, true);
//                     break;

//                 case State.ATTACK:
//                     anim.SetBool(hashIsAttack, true);
//                     break;

//                 case State.DIE:
//                     anim.SetTrigger(hashDie);
//                     GetComponent<CapsuleCollider>().enabled = false;
//                     agent.isStopped = true;
//                     isDie = true;
//                     break;
//             }
//             yield return new WaitForSeconds(0.3f);
//         }
//     }

//     IEnumerator CheckMonsterState()
//     {
//         while (isDie == false)
//         {
//             if (state == State.DIE) yield break;

//             float distance = Vector3.Distance(monsterTr.position, playerTr.position);
//             state = State.IDLE;

//             if (distance <= traceDist)
//             {
//                 state = State.TRACE;
//             }

//             if (distance <= attackDist)
//             {
//                 state = State.ATTACK;
//             }
//             yield return new WaitForSeconds(0.3f);
//         }
//     }

//     void OnCollisionEnter(Collision coll)
//     {
//         if (coll.collider.CompareTag("BULLET"))
//         {
//             Destroy(coll.gameObject);
//             anim.SetTrigger(hashHit);

//             hp -= 20;
//             UpdateHealthBar();  // 체력바 업데이트

//             if (hp <= 0)
//             {
//                 state = State.DIE;
//             }
//         }
//     }

//     public void OnDamage()
//     {
//         anim.SetTrigger(hashHit);

//         hp -= 20;
//         UpdateHealthBar();  // 체력바 업데이트

//         if (hp <= 0)
//         {
//             state = State.DIE;
//         }
//     }

//     private void UpdateHealthBar()
//     {
//         if (healthBar != null)
//         {
//             healthBar.fillAmount = (float)hp / maxHp;
//         }
//     }

//     public void YouWin()
//     {
//         StopAllCoroutines();
//         agent.isStopped = true;

//         anim.SetTrigger("PlayerDie");
//     } 
// }
