using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public Player Player { get; private set; }
    public PlayerController Controller { get; private set; }
    public StateProvider StateProvider { get; private set; }
    public Animator Animator { get; private set; }
    public BaseState CurrentState { get; set; }

    private void Awake()
    {
        Player = GetComponent<Player>();
        Controller = GetComponent<PlayerController>();
        StateProvider = new StateProvider(this);
        Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        CurrentState?.UpdateStates();
    }

    private void Initialize()
    {
        CurrentState = StateProvider.GetState(PlayerStates.Ground);
        CurrentState?.EnterState();
    }
}
