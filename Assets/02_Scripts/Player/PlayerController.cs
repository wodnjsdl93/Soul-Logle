using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputs playerInputs { get; private set; }
    public PlayerInputs.PlayerActions playerActions { get; private set; }

    public Camera playerCamera; // 플레이어가 사용하는 카메라
    public float rotationSpeed = 10f; // 좌우 회전 속도
    public float pitchSpeed = 5f; // 상하 회전 속도
    public float maxPitch = 80f; // 상하 회전의 최대 각도
    public float minPitch = -80f; // 상하 회전의 최소 각도

    private float pitch = 0f; // 현재 상하 각도

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        playerActions = playerInputs.Player;
        
    }

    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }

    private void Update()
    {
        RotatePlayerTowardsMouse();
    }

    private void RotatePlayerTowardsMouse()
    {
        // 마우스 이동량 가져오기
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x;
        float mouseY = mouseDelta.y;

        // 상하 회전 계산
        pitch -= mouseY * pitchSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); // 피치 각도 제한

        // 좌우 회전 계산
        Quaternion yawRotation = Quaternion.Euler(0, mouseX * rotationSpeed * Time.deltaTime, 0);

        // 플레이어 상하 회전
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);

        // 플레이어 좌우 회전 (플레이어가 바라보는 방향)
        transform.rotation = yawRotation * transform.rotation;
    }
}
