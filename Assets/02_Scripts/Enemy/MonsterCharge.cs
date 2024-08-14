using UnityEngine;

public class MonsterCharge : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public float speed = 5f; // 몬스터의 속도
    public float chargeDistance = 10f; // 돌진 시작 거리
    public float chargeDuration = 2f; // 돌진 지속 시간

    private bool isCharging = false;
    private float chargeTime = 0f;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기
    }

    private void Update()
    {
        // 플레이어와의 거리 계산
        float distance = Vector3.Distance(transform.position, player.position);

        // 플레이어가 돌진 범위에 들어오면 돌진 시작
        if (distance <= chargeDistance && !isCharging)
        {
            StartCharge();
        }

        if (isCharging)
        {
            ChargeTowardsPlayer();

            // 돌진 시간이 끝나면 돌진 종료
            chargeTime += Time.deltaTime;
            if (chargeTime >= chargeDuration)
            {
                StopCharge();
            }
        }
    }

    private void StartCharge()
    {
        isCharging = true;
        chargeTime = 0f;
        animator.SetBool("isCharging", true); // 애니메이션 전환
    }

    private void ChargeTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    private void StopCharge()
    {
        isCharging = false;
        animator.SetBool("isCharging", false); // 애니메이션 전환
    }
}
