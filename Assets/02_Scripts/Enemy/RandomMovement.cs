using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    public float moveRange = 10f; // 이동 범위
    public float moveSpeed = 5f;  // 이동 속도
    public float changeDirectionInterval = 2f; // 방향 변경 간격
    public float stopDurationMin = 1f; // 최소 멈춤 시간
    public float stopDurationMax = 3f; // 최대 멈춤 시간

    private Vector3 targetPosition;
    private float changeDirectionTimer;
    private float stopTimer;
    private bool isMoving = true; // 현재 이동 중인지 여부
    private Animator animator; // Animator 변수 추가

    void Start()
    {
        // Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();
        // 초기 목표 위치 설정
        SetRandomTargetPosition();
        changeDirectionTimer = changeDirectionInterval;
        stopTimer = GetRandomStopDuration();
    }

    void Update()
    {
        // 타이머 감소
        changeDirectionTimer -= Time.deltaTime;
        stopTimer -= Time.deltaTime;

        if (isMoving)
        {
            // 목표 위치로 이동
            MoveTowardsTarget();

            // 이동 상태에 따라 애니메이션 상태 변경
            if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                // 걷기 애니메이션 실행
                animator.SetBool("isWalking", true);
            }
            else
            {
                // 걷기 애니메이션 중지
                animator.SetBool("isWalking", false);

                // 새로운 목표 위치 설정
                if (changeDirectionTimer <= 0)
                {
                    SetRandomTargetPosition();
                    changeDirectionTimer = changeDirectionInterval;
                }
            }

            // 멈추기 타이머가 끝났으면 멈추기
            if (stopTimer <= 0)
            {
                isMoving = false;
                animator.SetBool("isWalking", false);
                stopTimer = GetRandomStopDuration(); // 다음 멈춤 시간 설정
            }
        }
        else
        {
            // 멈춤 상태일 때 타이머가 끝나면 이동 시작
            if (stopTimer <= 0)
            {
                isMoving = true;
                SetRandomTargetPosition(); // 이동할 새로운 목표 설정
                changeDirectionTimer = changeDirectionInterval; // 방향 변경 타이머 리셋
                stopTimer = GetRandomStopDuration(); // 다음 멈춤 시간 설정
            }
        }
    }

    void SetRandomTargetPosition()
    {
        float x = Random.Range(-moveRange, moveRange);
        float y = Random.Range(-moveRange, moveRange);
        float z = Random.Range(-moveRange, moveRange);
        targetPosition = new Vector3(x, y, z);
    }

    void MoveTowardsTarget()
    {
        float step = moveSpeed * Time.deltaTime;
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        // 이동 방향으로 회전
        if (step > 0.01f) // 이동할 때만 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step * moveSpeed);
        }
    }

    float GetRandomStopDuration()
    {
        return Random.Range(stopDurationMin, stopDurationMax);
    }
}
