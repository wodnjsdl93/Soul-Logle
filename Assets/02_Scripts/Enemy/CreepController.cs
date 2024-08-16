using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreepController : MonoBehaviour
{
    public enum State { IDLE, TRACE, ATTACK, DIE, CHARGE };

    public State state = State.IDLE;

    public Slider hpSlider;

    [SerializeField] private float attackDist = 10.0f;
    [SerializeField] private float traceDist = 20.0f;
    [SerializeField] private float moveSpeed = 4f;

    [SerializeField] private float BossinitHp = 300.0f;
    [SerializeField] private float BosscurrHp = 300.0f;

    [SerializeField] private float moveRange = 10f;
    [SerializeField] private float changeDirectionInterval = 2f;
    [SerializeField] private float stopDurationMin = 1f;
    [SerializeField] private float stopDurationMax = 3f;
    [SerializeField] private float obstacleDetectionDistance = 5f;
    [SerializeField] private LayerMask obstacleLayer;

    [SerializeField] private float chargeDistance = 10f;
    [SerializeField] private float chargeDuration = 2f;
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeDamage = 20f; // Damage during charge

    private Transform playerTr;
    private Transform monsterTr;
    private NavMeshAgent agent;
    private Animator anim;

    private bool isDie = false;
    private bool isMoving = true;
    private bool isCharging = false;

    private Vector3 targetPosition;
    private float changeDirectionTimer;
    private float stopTimer;
    private float chargeTime = 0f;

    private Vector3 chargeDirection; // 돌진할 방향을 저장

    // Hash parameters
    private readonly int hashIsTrace = Animator.StringToHash("IsTrace");
    private readonly int hashIsAttack = Animator.StringToHash("IsAttack");
    private readonly int hashIsCharging = Animator.StringToHash("IsCharging");
    private readonly int hashDie = Animator.StringToHash("Die");

    void Start()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("PLAYER")?.GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());

        BosscurrHp = BossinitHp;
        hpSlider.maxValue = BossinitHp;
        hpSlider.value = BosscurrHp;

        // Random Movement 초기화
        SetRandomTargetPosition();
        changeDirectionTimer = changeDirectionInterval;
        stopTimer = GetRandomStopDuration();
    }

    void Update()
{
    // 돌진 중에는 다른 상태 처리를 하지 않음
    if (isCharging)
    {
        ChargeTowardsDirection(); // 특정 방향으로 돌진
        chargeTime += Time.deltaTime;
        
        if (chargeTime >= chargeDuration)
        {
            StopCharge(); // 일정 시간이 지나면 돌진 중지
        }
        return; // 돌진 중에는 다른 코드 실행하지 않음
    }
    
    // 돌진 중이 아닐 때에만 상태에 따라 이동 처리
    if (state == State.TRACE || state == State.ATTACK || state == State.CHARGE)
    {
        HandleChaseOrAttack(); // 추적, 공격 또는 돌진 처리
    }
    else if (state == State.IDLE)
    {
        HandleMovement(); // 대기 상태에서의 이동 처리
    }

    // 테스트용 입력 처리
    if (Input.GetKeyDown(KeyCode.Space))
    {
        TakeDamage(10);
    }

    if (Input.GetKeyDown(KeyCode.H))
    {
        Heal(5);
    }
}


    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    agent.isStopped = true;
                    anim.SetBool(hashIsTrace, false);
                    anim.SetBool(hashIsAttack, false);
                    anim.SetBool(hashIsCharging, false);
                    break;

                case State.TRACE:
                    agent.isStopped = false;
                    agent.SetDestination(playerTr.position);
                    anim.SetBool(hashIsTrace, true);
                    anim.SetBool(hashIsAttack, false);
                    anim.SetBool(hashIsCharging, false);
                    break;

                case State.ATTACK:
                    agent.isStopped = true;
                    anim.SetBool(hashIsAttack, true);
                    anim.SetBool(hashIsTrace, false);
                    anim.SetBool(hashIsCharging, false);
                    break;

                case State.CHARGE:
                    anim.SetBool(hashIsCharging, true);
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
        while (!isDie)
        {
            if (state == State.DIE) yield break;

            float distance = Vector3.Distance(monsterTr.position, playerTr.position);

            if (distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if (distance <= chargeDistance && !isCharging)
            {
                state = State.CHARGE;
            }
            else if (distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (BosscurrHp > 0.0f && coll.gameObject.CompareTag("PUNCH"))
        {
            TakeDamage(10.0f);
        }
    }
    public void TakeDamage(float damage)
    {
        BosscurrHp -= damage;
        BosscurrHp = Mathf.Clamp(BosscurrHp, 0, BossinitHp);
        hpSlider.value = BosscurrHp;
        if (BosscurrHp <= 0)
        {
            state = State.DIE;
        }
    }

    public void Heal(float amount)
    {
        BosscurrHp += amount;
        BosscurrHp = Mathf.Clamp(BosscurrHp, 0, BossinitHp);
        hpSlider.value = BosscurrHp;
    }

    void HandleMovement()
    {
        if (state == State.ATTACK || state == State.TRACE || state == State.CHARGE) return;

        changeDirectionTimer -= Time.deltaTime;
        stopTimer -= Time.deltaTime;

        if (isMoving)
        {
            DetectAndAvoidObstacles();
            MoveTowardsTarget();

            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                anim.SetBool("isWalking", true);
            }
            else
            {
                anim.SetBool("isWalking", false);

                if (changeDirectionTimer <= 0)
                {
                    SetRandomTargetPosition();
                    changeDirectionTimer = changeDirectionInterval;
                    stopTimer = GetRandomStopDuration();
                }
            }

            if (stopTimer <= 0)
            {
                isMoving = false;
                anim.SetBool("isWalking", false);
                stopTimer = GetRandomStopDuration();
            }
        }
        else
        {
            if (stopTimer <= 0)
            {
                isMoving = true;
                SetRandomTargetPosition();
                changeDirectionTimer = changeDirectionInterval;
                stopTimer = GetRandomStopDuration();
            }
        }
    }

    void HandleChaseOrAttack()
    {
        if (state == State.CHARGE)
        {
            ChargeTowardsPlayer();
            chargeTime += Time.deltaTime;
            if (chargeTime >= chargeDuration)
            {
                StopCharge();
            }
        }
        else if (state == State.TRACE)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTr.position);
            anim.SetBool(hashIsTrace, true);
            anim.SetBool(hashIsAttack, false);
            anim.SetBool(hashIsCharging, false);
        }
        else if (state == State.ATTACK)
        {
            agent.isStopped = true;
            anim.SetBool(hashIsAttack, true);
            anim.SetBool(hashIsTrace, false);
            anim.SetBool(hashIsCharging, false);
        }
    }

    void SetRandomTargetPosition()
    {
        float x = Random.Range(-moveRange, moveRange);
        float z = Random.Range(-moveRange, moveRange);
        float y = transform.position.y;
        targetPosition = new Vector3(x, y, z);
    }

    void MoveTowardsTarget()
    {
        float step = moveSpeed * Time.deltaTime;
        Vector3 direction = (targetPosition - transform.position).normalized;

        // 이동 방향으로 회전
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step * moveSpeed);
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }

    void DetectAndAvoidObstacles()
    {
        RaycastHit hit;
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (Physics.Raycast(transform.position, direction, out hit, obstacleDetectionDistance, obstacleLayer))
        {
            Vector3 avoidanceDirection = Vector3.Reflect(direction, hit.normal);
            avoidanceDirection.y = 0;
            targetPosition = transform.position + avoidanceDirection * moveRange;
        }
    }

    float GetRandomStopDuration()
    {
        return Random.Range(stopDurationMin, stopDurationMax);
    }
[SerializeField] private string tagToIgnore = "PLAYER";


private List<Collider> ignoredColliders = new List<Collider>();

void StartCharge()
{
    isCharging = true;
    anim.SetBool(hashIsCharging, true);
    
    // NavMeshAgent 경로 탐색 중지
    agent.isStopped = true;

    // 돌진할 방향 설정 (현재 바라보는 방향)
    chargeDirection = transform.forward; // 현재 바라보는 방향으로 돌진

    // 돌진 시간을 초기화
    chargeTime = 0f;
}


void ChargeTowardsDirection()
{
    // 돌진 방향으로 이동
    float step = chargeSpeed * Time.deltaTime;

    // 충돌 처리를 고려하여 이동
    if (Physics.Raycast(transform.position, chargeDirection, out RaycastHit hit, step, obstacleLayer))
    {
        // 충돌이 감지되면 방향을 조정하거나, 이동을 멈출 수 있음
        // 여기서는 충돌 처리 코드를 생략
    }
    else
    {
        // 충돌이 없으면 이동
        transform.position += chargeDirection * step;
    }
}

void ChargeTowardsPlayer()
{
    isCharging = true;
    agent.isStopped = true; // NavMeshAgent의 경로 탐색을 중지

    // 돌진 방향으로 이동
    Vector3 direction = (playerTr.position - transform.position).normalized;
    float step = chargeSpeed * Time.deltaTime;

    // Collider와 NavMeshAgent 모두를 수동으로 이동
    transform.position = Vector3.MoveTowards(transform.position, playerTr.position, step);
}

void StopCharge()
{
    isCharging = false;
    anim.SetBool(hashIsCharging, false);

    // NavMeshAgent 다시 활성화
    agent.isStopped = false;
    agent.ResetPath(); // 경로 초기화

    // 돌진 후 상태 전환 로직
    float distanceToPlayer = Vector3.Distance(transform.position, playerTr.position);

    if (distanceToPlayer <= attackDist)
    {
        state = State.ATTACK;
    }
    else if (distanceToPlayer <= traceDist)
    {
        state = State.TRACE;
    }
    else
    {
        state = State.IDLE;
    }
}



void IgnoreCollisionWithTag(bool shouldIgnore)
{
    Collider[] colliders = FindObjectsOfType<Collider>();

    foreach (Collider coll in colliders)
    {
        if (coll.CompareTag(tagToIgnore))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), coll, shouldIgnore);

            if (shouldIgnore)
            {
                ignoredColliders.Add(coll);
            }
            else
            {
                ignoredColliders.Remove(coll);
            }
        }
    }
} 
}