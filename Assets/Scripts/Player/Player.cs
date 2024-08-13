using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public PlayerSO Data { get; private set;}
    [field: Header("Animations")]
    [field: SerializeField] public PlayerAnimationData AnimationData { get; private set; }

    public Animator Animator { get; private set; }
    public PlayerController Input { get; private set; }
    public CharacterController Controller { get; private set; }
    public ForceReceiver ForceReceiver { get; private set; }

    private PlayerStateMachine stateMachine;

    private void Awake()
    {
        AnimationData.Initialize();
        Animator = GetComponentInChildren<Animator>();
        Input = GetComponent<PlayerController>();
        Controller = GetComponent<CharacterController>();
        ForceReceiver = GetComponent<ForceReceiver>();

        if (Animator == null)
        {
            Debug.LogError("Animator component is not found.");
        }
        if (Input == null)
        {
            Debug.LogError("PlayerController component is not found.");
        }
        if (Controller == null)
        {
            Debug.LogError("CharacterController component is not found.");
        }
        if (ForceReceiver == null)
        {
            Debug.LogError("ForceReceiver component is missing.");
        }
        
        stateMachine = new PlayerStateMachine(this);
        stateMachine.ChangeState(stateMachine.IdleState);

        Debug.Log("Player script enabled: " + this.enabled);
        Debug.Log("Player GameObject active: " + gameObject.activeSelf);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() 
    {
        if (stateMachine != null)
        {
            stateMachine.HandleInput();
            stateMachine.Update();
        }
    }

    private void FixedUpdate() 
    {
        if (stateMachine != null)
        {
            stateMachine.PhysicsUpdate();    
        }
    }

    private void OnEnable()
    {
        Debug.Log("Player script has been enabled.");
        
    }

    private void OnDisable()
    {
        Debug.Log("Player script has been disabled.");
    Debug.Log("StateMachine is " + (stateMachine != null ? "still active." : "null."));
    }
}
