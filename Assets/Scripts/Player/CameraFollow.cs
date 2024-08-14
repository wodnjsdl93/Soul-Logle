using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // 캐릭터의 Transform
    public Transform playerCamera;    // 카메라의 Transform
    public float distance = 5f;       // 캐릭터와 카메라 간의 거리
    public float height = 3f;         // 카메라의 높이
    public float smoothSpeed = 0.125f; // 카메라의 부드러운 이동 속도
    public float rotationSpeed = 5f;  // 카메라의 회전 속도
    public LayerMask collisionMask;   // 충돌을 검사할 레이어
    public float collisionOffset = 0.5f; // 충돌 감지 시 카메라가 이동할 오프셋
    public float verticalRotationSpeed = 2f; // 수직 회전 속도
    public float verticalAngleLimit = 45f;   // 수직 회전 제한 각도

    private Vector3 dollyDir; // 카메라와 캐릭터 사이의 방향
    private float verticalAngle = 0f; // 카메라의 수직 회전 각도

    private void LateUpdate()
    {
        // 카메라의 목표 위치 계산
        Vector3 desiredPosition = player.position - player.forward * distance + Vector3.up * height;
        Vector3 smoothedPosition = Vector3.Lerp(playerCamera.position, desiredPosition, smoothSpeed);
        playerCamera.position = smoothedPosition;

        // 카메라가 캐릭터를 바라보도록 회전
        Quaternion desiredRotation = Quaternion.LookRotation(player.position - playerCamera.position);
        Quaternion smoothedRotation = Quaternion.Slerp(playerCamera.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        playerCamera.rotation = smoothedRotation;

        // 충돌 감지 및 카메라 위치 조정
        dollyDir = player.position - playerCamera.position;
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, dollyDir, out hit, distance + collisionOffset, collisionMask))
        {
            playerCamera.position = hit.point - dollyDir.normalized * collisionOffset;
        }
    }

    private void Update()
    {
        // 수직 회전 조절
        float verticalInput = Input.GetAxis("Mouse Y");
        verticalAngle -= verticalInput * verticalRotationSpeed;
        verticalAngle = Mathf.Clamp(verticalAngle, -verticalAngleLimit, verticalAngleLimit);

        // 카메라의 수직 회전 적용
        playerCamera.localRotation = Quaternion.Euler(verticalAngle, playerCamera.localRotation.eulerAngles.y, 0f);
    } 
}
