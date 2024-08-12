using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public PlayerSO Data { get; private set;}
    [field: Header("Animations")]
    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }

    public Animator Animator { get; private set; }
    public PlayerController Input { get; private set; }
    public CharacterController Controller { get; private set; }

    private PlayerStateMachine StateMachine;

    private void Awake()
    {
        AnimationData.Initialize();
		Animator = GetComponentInChildren<Animator>();
		Input = GetComponent<PlayerController>();
		Controller = GetComponent<CharacterController>();

        stateMachine = new PlayerStateMachine(this);
        stateMachine.ChangState(stateMachine.IdleState);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() 
    {
        stateMachine.HandleInput();
        stateMachine.Update();
    }

    private void FixedUpdate() 
    {
        stateMachine.PhysicsUpdate();    
    }

}