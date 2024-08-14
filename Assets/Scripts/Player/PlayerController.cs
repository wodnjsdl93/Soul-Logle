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
    public float movementSpeed = 5f; // 플레이어 이동 속도

    private float pitch = 0f; // 현재 상하 각도
    private float yaw = 0f; // 현재 좌우 각도
    private Vector2 movementInput; // 이동 입력

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        playerActions = playerInputs.Player;
    }

    private void OnEnable()
    {
        playerInputs.Enable();
        playerActions.Movement.performed += OnMovementInput;
        playerActions.Movement.canceled += OnMovementInput;
    }

    private void OnDisable()
    {
        playerInputs.Disable();
        playerActions.Movement.performed -= OnMovementInput;
        playerActions.Movement.canceled -= OnMovementInput;
    }

    private void Update()
    {
        RotateCamera();
        MovePlayer();
    }

    private void OnMovementInput(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void RotateCamera()
    {
        // 마우스 이동량 가져오기
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x;
        float mouseY = mouseDelta.y;

        // 상하 회전 계산
        pitch -= mouseY * pitchSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch); // 피치 각도 제한

        // 좌우 회전 계산
        yaw += mouseX * rotationSpeed * Time.deltaTime;

        // 카메라의 상하 회전
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);

        // 플레이어의 좌우 회전
        transform.rotation = Quaternion.Euler(0, yaw, 0);
    }

    private void MovePlayer()
    {
        // 카메라의 방향을 기준으로 이동 방향 계산
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        // Y 축 방향 벡터를 0으로 설정하여 수평 이동만 고려
        forward.y = 0;
        right.y = 0;

        // 방향 벡터 정규화
        forward.Normalize();
        right.Normalize();

        // 이동 방향 계산
        Vector3 moveDirection = (forward * movementInput.y + right * movementInput.x).normalized;

        // 플레이어 이동 처리
        transform.position += moveDirection * movementSpeed * Time.deltaTime;
    }
}
