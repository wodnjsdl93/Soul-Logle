using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossAI : MonoBehaviour
{
    public enum BossState { Idle, Moving, Chasing, Attacking }
    public BossState currentState = BossState.Idle;

    public float moveSpeed = 3.0f;
    public float chaseRange = 10.0f;
    public float attackRange = 2.0f;
    public Transform[] randomPoints;
    public Text stateText;
    private Transform player;
    private int currentPointIndex = 0;
    private bool isChasingPlayer = false;
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("PLAYER").transform;
        animator = GetComponent<Animator>();
        StartCoroutine(RandomMovement());
        UpdateStateText();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            StopCoroutine(RandomMovement());
            isChasingPlayer = true;
            currentState = BossState.Chasing;
            ChasePlayer();
        }
        else if (!isChasingPlayer)
        {
            currentState = BossState.Moving;
        }

        // 공격 범위에 있는지 확인
        if (isChasingPlayer && distanceToPlayer <= attackRange)
        {
            currentState = BossState.Attacking;
            AttackPlayer();
        }

        UpdateStateText();
    }

    void ChasePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        transform.LookAt(player.position);
        animator.SetBool("isWalking", true);
    }

    IEnumerator RandomMovement()
    {
        while (!isChasingPlayer)
        {
            Transform targetPoint = randomPoints[currentPointIndex];
            while (Vector3.Distance(transform.position, targetPoint.position) > 0.2f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);
                transform.LookAt(targetPoint.position);
                currentState = BossState.Moving;
                animator.SetBool("isWalking", true);
                yield return null;
            }
            animator.SetBool("isWalking", false);
            currentState = BossState.Idle;

            currentPointIndex = (currentPointIndex + 1) % randomPoints.Length;

            yield return new WaitForSeconds(2.0f);
        }
    }

    void AttackPlayer()
    {
        // 공격 상태로 전환된 후에야 공격 애니메이션 실행
        animator.SetBool("isWalking", false);

        int attackType = Random.Range(0, 2);

        if (attackType == 0)
        {
            animator.SetTrigger("Attack1");
            Debug.Log("보스가 첫 번째 공격을 합니다."); // 디버그 로그로 확인
        }
        else
        {
            animator.SetTrigger("Attack2");
            Debug.Log("보스가 두 번째 공격을 합니다."); // 디버그 로그로 확인
        }
    }

    void UpdateStateText()
    {
        if (stateText != null)
        {
            string actionText = "";

            switch (currentState)
            {
                case BossState.Idle:
                    actionText = "대기 중";
                    break;
                case BossState.Moving:
                    actionText = "이동 중";
                    break;
                case BossState.Chasing:
                    actionText = "플레이어 추적 중";
                    break;
                case BossState.Attacking:
                    actionText = "공격 중";
                    break;
            }

            stateText.text = "보스 상태: " + actionText;
        }

        Debug.Log("Boss Current Action: " + currentState.ToString());
    }
}
