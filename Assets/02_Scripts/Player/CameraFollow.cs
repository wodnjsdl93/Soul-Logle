using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public Vector3 offset; // 카메라와 플레이어 사이의 거리
    public float smoothSpeed = 0.125f; // 카메라의 부드러운 추적 속도

    private void LateUpdate()
    {
        // 카메라의 목표 위치 계산
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 카메라가 플레이어를 바라보도록 설정
        transform.LookAt(player.position + player.forward);
    }
}
