using Godot;
using System;
using Softjam2023.Scripts.StateMachine;

public partial class MainPlayerController : CharacterBody3D
{
    [Signal]
    public delegate void WalkEventHandler();
    [Signal]
    public delegate void RunEventHandler();
    [Signal]
    public delegate void JumpEventHandler();


    [Export]
    public const float Speed = 5.0f;

    [Export]
    public const float SprintSpeed = 9.0f;

    [Export]
    public GameTime GameTime;

    public Node3D rifleContainer;
    public Rifle rifleWeapon;
    public InteractRifle interactRifle;
    public Node3D aimPoint;
    public Camera3D camera;
    private PlayerGameStateManager _gameStateManager;

    public const float JumpVelocity = 4.5f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    private bool _isSprinting = false;
    private bool _isJumping = false;

    public override void _Ready()
    {
        aimPoint = GetNode<Node3D>("HeadTilt/AimPoint");
        rifleContainer = GetNode<Node3D>("Visuals/rifle");
        camera = GetNode<Camera3D>("HeadTilt/Camera3D");
        rifleWeapon = rifleContainer.GetChild<Rifle>(0);
        interactRifle = GetNode<InteractRifle>("Scripts/InteractRifle");
        _gameStateManager = GetNode<PlayerGameStateManager>("Scripts/PlayerGameStateManager");
        camera.MakeCurrent();
    }

    public override void _PhysicsProcess(double delta)
    {
        float speed = Speed;
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y -= gravity * (float)delta;

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
            EmitSignal(SignalName.Jump);
        }

        if (Input.IsActionPressed("player_sprint"))
        {
            speed = SprintSpeed;
            _isSprinting = true;
        }
        else
        {
            _isSprinting = false;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector(
            "player_movement_left",
            "player_movement_right",
            "player_movement_forward",
            "player_movement_backward");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * speed;
            velocity.Z = direction.Z * speed;

            if (IsOnFloor())
            {

                if (_isSprinting)
                {
                    EmitSignal(SignalName.Run);
                }
                else
                {
                    EmitSignal(SignalName.Walk);
                }
            }
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public void GameStateChange(int newState, int oldState)
    {
        _gameStateManager.StateChange((GameStates)newState, (GameStates)oldState);
    }
}
